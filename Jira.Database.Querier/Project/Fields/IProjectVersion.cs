using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Project.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace lazyzu.Jira.Database.Querier.Project.Fields
{
    public interface IProjectVersion : IEquatable<IProjectVersion>
    {
        decimal Id { get; }
        string Name { get; }
        string Description { get; }
        bool Archived { get; }
        DateTime? StartDate { get; }
        DateTime? ReleaseDate { get; }
    }

    public interface IFullProjectVersion : IProjectVersion
    {
        IJiraProject Project { get; }
    }

    public class ProjectVersion : IProjectVersion
    {
        public decimal Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public bool Archived { get; init; }

        public DateTime? StartDate { get; init; }

        public DateTime? ReleaseDate { get; init; }

        public static bool operator ==(ProjectVersion left, IProjectVersion right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(ProjectVersion left, IProjectVersion right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IProjectVersion);
        }

        public bool Equals(IProjectVersion other)
        {
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Description})";
        }
    }

    public class FullProjectVersion : ProjectVersion, IFullProjectVersion
    {
        public IJiraProject Project { get; internal set; }

        public FullProjectVersion(IProjectVersion projectVersion, IJiraProject project)
        {
            this.Id = projectVersion.Id;
            this.Name = projectVersion.Name;
            this.Description = projectVersion.Description;
            this.Archived = projectVersion.Archived;
            this.StartDate = projectVersion.StartDate;
            this.ReleaseDate = projectVersion.ReleaseDate;
            this.Project = project;
        }
    }

    public static class ProjectVersionExtension
    {
        public static async Task<Dictionary<decimal, IProjectVersion[]>> LoadIssueVersionMap(decimal?[] issueIds, string assolication, JiraContext jiraContext, CancellationToken cancellationToken = default)
        {
            var query = from nodeassociation in jiraContext.nodeassociation.AsNoTracking()
                        where issueIds.Contains(nodeassociation.SOURCE_NODE_ID)
                           && nodeassociation.SOURCE_NODE_ENTITY == "Issue"
                           && nodeassociation.ASSOCIATION_TYPE == assolication
                        select new
                        {
                            nodeassociation.SOURCE_NODE_ID,
                            nodeassociation.SINK_NODE_ID
                        };

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var versionIds = queryResult.Select(dbModel => dbModel.SINK_NODE_ID).Distinct().ToArray();
            var versionMap = await ProjectVersionExtension.LoadVersionMap(versionIds, jiraContext, cancellationToken).ConfigureAwait(false);

            return queryResult.GroupBy(dbModel => dbModel.SOURCE_NODE_ID)
                .ToDictionary(issueIdGroup => issueIdGroup.Key
                            , issueIdGroup =>
                            {
                                var versionIds = issueIdGroup.Select(dbModel => dbModel.SINK_NODE_ID);
                                return LoadEntitiesFromId(versionIds, versionMap).ToArray();
                            });
        }

        public static IEnumerable<TEntity> LoadEntitiesFromId<TEntity>(IEnumerable<decimal> ids, Dictionary<decimal, TEntity> map)
        {
            foreach (var id in ids)
            {
                if (map.TryGetValue(id, out var component)) yield return component;
            }
        }

        public static async Task<Dictionary<decimal, IProjectVersion>> LoadVersionMap(decimal[] versionIds, JiraContext jiraContext, CancellationToken cancellationToken = default)
        {
            var query = from version in jiraContext.projectversion.AsNoTracking()
                        where versionIds.Contains(version.ID)
                        select new
                        {
                            version.PROJECT,
                            version.ID,
                            version.vname,
                            version.DESCRIPTION,
                            version.ARCHIVED,
                            version.STARTDATE,
                            version.RELEASEDATE
                        };

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.ToDictionary(dbModel => dbModel.ID, dbModel => new ProjectVersion
            {
                Id = dbModel.ID,
                Name = dbModel.vname,
                Description = dbModel.DESCRIPTION,
                Archived = ParseVersionArchivedBoolean(dbModel.ARCHIVED),
                StartDate = dbModel.STARTDATE,
                ReleaseDate = dbModel.RELEASEDATE
            } as IProjectVersion);
        }

        public static bool ParseVersionArchivedBoolean(string archivedStr)
        {
            return "true".Equals(archivedStr, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class ProjectVersionProjection : IProjectExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectVersionProjection(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectVersion
            };
        }

        public virtual async Task Projection(IEnumerable<JiraProject> projects, CancellationToken cancellationToken = default)
        {
            var _projects = projects?.ToArray() ?? new JiraProject[0];

            if (_projects.Any())
            {
                var projectIds = _projects.Select<JiraProject, decimal?>(project => project.Id).ToArray();

                var projectIdVersionMap = await LoadProjectIdVersionMap(projectIds, cancellationToken).ConfigureAwait(false);

                foreach (var project in _projects)
                {
                    if (projectIdVersionMap.TryGetValue(project.Id, out var versions))
                    {
                        project.Versions = versions.Select(version => new FullProjectVersion(version, project)).ToArray();
                    }
                    else project.Versions = new IFullProjectVersion[0];
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, ProjectVersion[]>> LoadProjectIdVersionMap(decimal?[] projectIds, CancellationToken cancellationToken = default)
        {
            var query = jiraContext.projectversion.AsNoTracking()
                .Where(projectversion => projectIds.Contains(projectversion.PROJECT))
                .OrderBy(projectversion => projectversion.SEQUENCE)
                .Select(projectversion => new
                {
                    projectversion.PROJECT,
                    projectversion.ID,
                    projectversion.vname,
                    projectversion.DESCRIPTION,
                    projectversion.ARCHIVED,
                    projectversion.STARTDATE,
                    projectversion.RELEASEDATE
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.GroupBy(dbModel => dbModel.PROJECT.Value)
                .ToDictionary(projectIdGroup => projectIdGroup.Key
                , projectIdGroup => projectIdGroup.Select(dbModel => new ProjectVersion
                {
                    Id = dbModel.ID,
                    Name = dbModel.vname,
                    Description = dbModel.DESCRIPTION,
                    Archived = ProjectVersionExtension.ParseVersionArchivedBoolean(dbModel.ARCHIVED),
                    StartDate = dbModel.STARTDATE,
                    ReleaseDate = dbModel.RELEASEDATE
                }).ToArray());
        }
    }
}
