using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Project.Contract;
using lazyzu.Jira.Database.Querier.Project.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Fields
{
    public interface IProjectRole
    {
        decimal Id { get; }
        string Name { get; }
        string Description { get; }
    }

    [Equatable(Explicit = true)]
    public partial class ProjectRole : IProjectRole
    {
        [DefaultEquality]
        public decimal Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Description})";
        }
    }

    public interface IProjectRoleActorMap : IProjectRole
    {
        HashSet<IProjectRoleActor> Actors { get; }
    }

    [Equatable(Explicit = true)]
    public partial class ProjectRoleActorMap : ProjectRole, IProjectRoleActorMap
    {
        [DefaultEquality]
        private decimal _id => Id;

        [SetEquality]
        public HashSet<IProjectRoleActor> Actors { get; init; }
    }

    public interface IProjectRoleActor
    {
        string Type { get; }
        string Value { get; }

        public static class TypeSelection
        {
            public const string AtlassianGroupRoleActor = "atlassian-group-role-actor";
            public const string AtlassianUserRoleActor = "atlassian-user-role-actor";
        }
    }

    [Equatable]
    public partial class ProjectRoleActor : IProjectRoleActor
    {
        public string Type { get; init; }
        public string Value { get; init; }

        public override string ToString()
        {
            return $"{Value} ({Type})";
        }
    }

    public class ProjectRoleProjection : IProjectExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly Func<IProjectRoleService> projectRoleServiceGetter;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectRoleProjection(JiraContext jiraContext, Func<IProjectRoleService> projectRoleServiceGetter, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.projectRoleServiceGetter = projectRoleServiceGetter;
            this.cache = cache;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectRole
            };
        }

        public ProjectRoleProjection(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache, ILogger logger)
            : this(jiraContext, () => jiraDatabaseQuerierGetter().Project.ProjectRole, cache, logger)
        { }

        public virtual async Task Projection(IEnumerable<JiraProject> projects, CancellationToken cancellationToken = default)
        {
            var _projects = projects?.ToArray() ?? new JiraProject[0];

            if (_projects.Any())
            {
                var projectIds = _projects.Select<JiraProject, decimal?>(project => project.Id).ToArray();

                var projectIdRoleActorMap = await BuildProjectIdRoleActorMap(projectIds, cancellationToken).ConfigureAwait(false);

                if (projectIdRoleActorMap.Any())
                {
                    foreach (var project in _projects)
                    {
                        if (projectIdRoleActorMap.TryGetValue(project.Id, out var roleActorMaps)) project.ProjectRoles = roleActorMaps;
                        else project.ProjectRoles = new IProjectRoleActorMap[0];
                    }
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, IProjectRoleActorMap[]>> BuildProjectIdRoleActorMap(decimal?[] projectIds, CancellationToken cancellationToken = default)
        {
            if (projectIds.Any())
            {
                var query = jiraContext.projectroleactor.AsNoTracking()
                .Where(projectroleactor => projectIds.Contains(projectroleactor.PID))
                .Select(projectroleactor => new
                {
                    projectroleactor.PID,
                    projectroleactor.PROJECTROLEID,
                    projectroleactor.ROLETYPE,
                    projectroleactor.ROLETYPEPARAMETER
                });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                IDictionary<decimal, IProjectRole> projectRoles = cache.ProjectRoles;
                if (projectRoles.Any() == false)
                {
                    var projectRoleService = projectRoleServiceGetter();
                    projectRoles = (await projectRoleService.GetProjectRolesAsync(cancellationToken).ConfigureAwait(false))
                        .ToDictionary(projectRole => projectRole.Id);
                }

                var result = new Dictionary<decimal, IProjectRoleActorMap[]>();
                foreach (var dbModelProjectIdGrouped in queryResult.GroupBy(dbModel => dbModel.PID))
                {
                    var projectId = dbModelProjectIdGrouped.Key;

                    var roleIdActorMap = queryResult.GroupBy(projectroleactor => projectroleactor.PROJECTROLEID)
                        .ToDictionary(roleIdGroup => roleIdGroup.Key
                            , roleIdGroup => roleIdGroup.Select(dbModel => new ProjectRoleActor
                            {
                                Type = dbModel.ROLETYPE,
                                Value = dbModel.ROLETYPEPARAMETER
                            } as IProjectRoleActor).ToArray());

                    var projectRoleActorMap = ProjectRoleService.BindAcotrWithProjectRoleInfo(roleIdActorMap, projectRoles);

                    result.TryAdd(projectId.Value, projectRoleActorMap.ToArray());
                }
                return result;
            }
            else return new Dictionary<decimal, IProjectRoleActorMap[]>();
        }
    }
}
