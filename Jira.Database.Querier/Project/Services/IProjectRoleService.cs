using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Project.Fields;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Services
{
    public interface IProjectRoleService
    {
        Task<IEnumerable<IProjectRole>> GetProjectRolesAsync(CancellationToken cancellationToken = default);

        Task<IProjectRole[]> GetProjectRolesAsync(decimal projectId, CancellationToken cancellationToken = default);
    }

    public class ProjectRoleService : IProjectRoleService
    {
        protected readonly JiraContext jiraContext;
        protected readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectRoleService(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;
            this.logger = logger;
        }

        public virtual async Task<IEnumerable<IProjectRole>> GetProjectRolesAsync(CancellationToken cancellationToken = default)
        {
            if (cache.ProjectRoles.Any() == false)
            {
                var query = jiraContext.projectrole.AsNoTracking().Select<projectrole, IProjectRole>(projectrole => new ProjectRole
                {
                    Id = projectrole.ID,
                    Name = projectrole.NAME,
                    Description = projectrole.DESCRIPTION
                });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var prokectRole in queryResult)
                {
                    cache.ProjectRoles.TryAdd(prokectRole.Id, prokectRole);
                }
            }

            return cache.ProjectRoles.Values;
        }

        public virtual async Task<IProjectRole[]> GetProjectRolesAsync(string projectKey, CancellationToken cancellationToken = default)
        {
            var projectKeyService = jiraDatabaseQuerierGetter().Project.ProjectKey;
            var projectId = await projectKeyService.GetProjectIdAsync(projectKey, cancellationToken).ConfigureAwait(false);

            if (projectId.HasValue) return await GetProjectRolesAsync(projectId.Value, cancellationToken).ConfigureAwait(false);
            else return new IProjectRole[0];
        }

        public virtual async Task<IProjectRole[]> GetProjectRolesAsync(decimal projectId, CancellationToken cancellationToken = default)
        {
            var query = jiraContext.projectroleactor.AsNoTracking()
                .Where(projectroleactor => projectroleactor.PID == projectId)
                .Select(projectroleactor => new
                {
                    projectroleactor.PROJECTROLEID,
                    projectroleactor.ROLETYPE,
                    projectroleactor.ROLETYPEPARAMETER
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var roleActorMap = queryResult.GroupBy(projectroleactor => projectroleactor.PROJECTROLEID)
                .ToDictionary(roleIdGroup => roleIdGroup.Key
                            , roleIdGroup => roleIdGroup.Select(dbModel => new ProjectRoleActor
                            {
                                Type = dbModel.ROLETYPE,
                                Value = dbModel.ROLETYPEPARAMETER
                            } as IProjectRoleActor).ToArray());

            if (cache.ProjectRoles.Any() == false) await this.GetProjectRolesAsync(cancellationToken).ConfigureAwait(false);

            return BindAcotrWithProjectRoleInfo(roleActorMap, cache.ProjectRoles).ToArray();
        }

        public static IEnumerable<IProjectRoleActorMap> BindAcotrWithProjectRoleInfo(Dictionary<decimal?, IProjectRoleActor[]> projectRoleIdActorMaps, IDictionary<decimal, IProjectRole> projectRoleMap)
        {
            if (projectRoleIdActorMaps.Any())
            {
                foreach (var projectRoleIdActorMap in projectRoleIdActorMaps)
                {
                    if (projectRoleIdActorMap.Key.HasValue && projectRoleMap.TryGetValue(projectRoleIdActorMap.Key.Value, out var projectRole))
                    {
                        yield return new ProjectRoleActorMap
                        {
                            Id = projectRole.Id,
                            Name = projectRole.Name,
                            Description = projectRole.Description,
                            Actors = projectRoleIdActorMap.Value.ToHashSet()
                        };
                    }
                }
            }
        }
    }
}
