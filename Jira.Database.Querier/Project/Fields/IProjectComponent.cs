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
    public interface IProjectComponent : IEquatable<IProjectComponent>
    {
        decimal Id { get; }
        string Name { get; }
        string Description { get; }
        bool Archived { get; }
        bool Deleted { get; }
    }

    public interface IFullProjectComponent : IProjectComponent
    {
        IJiraProject Project { get; }
    }

    public class ProjectComponent : IProjectComponent
    {
        public decimal Id { get; init; }

        public string Name { get; init; }
        public string Description { get; init; }

        public bool Archived { get; init; }

        public bool Deleted { get; init; }

        public static bool operator ==(ProjectComponent left, IProjectComponent right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(ProjectComponent left, IProjectComponent right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IProjectComponent);
        }

        public bool Equals(IProjectComponent other)
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

    public class FullProjectComponent : ProjectComponent, IFullProjectComponent
    {
        public IJiraProject Project { get; internal set; }

        public FullProjectComponent()
        { }

        public FullProjectComponent(IProjectComponent component, IJiraProject project)
        {
            this.Id = component.Id;
            this.Name = component.Name;
            this.Description = component.Description;
            this.Archived = component.Archived;
            this.Deleted = component.Deleted;
            this.Project = project;
        }
    }

    internal class ProjectComponentExtension
    {
        public static async Task<Dictionary<decimal, IProjectComponent>> LoadComponentMap(decimal[] componentIds, JiraContext jiraContext, CancellationToken cancellationToken = default)
        {
            var query = jiraContext.component.AsNoTracking()
                .Where(c => componentIds.Contains(c.ID))
                .Select(c => new
                {
                    c.PROJECT,
                    c.ID,
                    c.cname,
                    c.description,
                    c.ARCHIVED,
                    c.DELETED
                });

            var queryResult = await query.ToArrayAsync(cancellationToken);

            return queryResult.ToDictionary(dbModel => dbModel.ID, dbModel => new ProjectComponent
            {
                Id = dbModel.ID,
                Name = dbModel.cname,
                Description = dbModel.description,
                Archived = ParseProjectComponentBoolean(dbModel.ARCHIVED),
                Deleted = ParseProjectComponentBoolean(dbModel.DELETED)
            } as IProjectComponent);
        }

        public static bool ParseProjectComponentBoolean(string boolStr)
        {
            if (string.IsNullOrEmpty(boolStr)) return false;
            return "Y".Equals(boolStr, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class ProjectComponentProjection : IProjectExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectComponentProjection(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectComponent
            };
        }

        public virtual async Task Projection(IEnumerable<JiraProject> projects, CancellationToken cancellationToken = default)
        {
            var _projects = projects?.ToArray() ?? new JiraProject[0];

            if (_projects.Any())
            {
                var projectIds = _projects.Select<JiraProject, decimal?>(project => project.Id).ToArray();
                var projectIdComponentMap = await LoadProjectIdComponentMap(projectIds, cancellationToken).ConfigureAwait(false);

                foreach (var project in _projects)
                {
                    if (projectIdComponentMap.TryGetValue(project.Id, out var components))
                    {
                        project.Components = components.Select(component => new FullProjectComponent(component, project)).ToArray();
                    }
                    else project.Components = new IFullProjectComponent[0];
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, ProjectComponent[]>> LoadProjectIdComponentMap(decimal?[] projectIds, CancellationToken cancellationToken = default)
        {
            var query = jiraContext.component.AsNoTracking()
                .Where(c => projectIds.Contains(c.PROJECT))
                .Select(c => new
                {
                    c.PROJECT,
                    c.ID,
                    c.cname,
                    c.description,
                    c.ARCHIVED,
                    c.DELETED
                });

            var queryResult = await query.ToArrayAsync(cancellationToken);

            return queryResult.GroupBy(dbModel => dbModel.PROJECT.Value)
                .ToDictionary(projectIdGroup => projectIdGroup.Key
                            , projectIdGroup => projectIdGroup.Select(dbModel => new ProjectComponent
                            {
                                Id = dbModel.ID,
                                Name = dbModel.cname,
                                Description = dbModel.description,
                                Archived = ProjectComponentExtension.ParseProjectComponentBoolean(dbModel.ARCHIVED),
                                Deleted = ProjectComponentExtension.ParseProjectComponentBoolean(dbModel.DELETED)
                            }).ToArray());
        }
    }
}
