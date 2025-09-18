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
    public interface IProjectCategory
    {
        decimal Id { get; }
        string Name { get; }
        string Description { get; }
    }

    [Equatable(Explicit = true)]
    public partial class ProjectCategory : IProjectCategory
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

    

    public class ProjectCategoryProjection : IProjectExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly Func<IProjectCategoryService> projectCategoryServiceGetter;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectCategoryProjection(JiraContext jiraContext, Func<IProjectCategoryService> projectCategoryServiceGetter, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.projectCategoryServiceGetter = projectCategoryServiceGetter;
            this.cache = cache;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectCategory
            };
        }

        public ProjectCategoryProjection(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache, ILogger logger)
            : this(jiraContext, () => jiraDatabaseQuerierGetter().Project.ProjectCategory, cache, logger)
        { }

        public virtual async Task Projection(IEnumerable<JiraProject> projects, CancellationToken cancellationToken = default)
        {
            var _projects = projects?.ToArray() ?? new JiraProject[0];

            if (_projects.Any())
            {
                var projectIds = _projects.Select<JiraProject, decimal?>(project => project.Id).ToArray();

                var projectCategoryIdQuery = from nodeassociation in jiraContext.nodeassociation.AsNoTracking()
                        where projectIds.Contains(nodeassociation.SOURCE_NODE_ID)
                                              && nodeassociation.SINK_NODE_ENTITY == "ProjectCategory"
                        select new
                        {
                            nodeassociation.SOURCE_NODE_ID,
                            nodeassociation.SINK_NODE_ID
                        };

                var projectCategoryIdQueryResult = (await projectCategoryIdQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(projectCategoryId => projectCategoryId.SOURCE_NODE_ID
                                , projectCategoryId => projectCategoryId.SINK_NODE_ID);

                var projectCategories = await LoadProjectCategories(cancellationToken).ConfigureAwait(false);

                foreach (var project in _projects)
                {
                    if (projectCategoryIdQueryResult.TryGetValue(project.Id, out var projectCategoryId))
                    {
                        if (projectCategories.TryGetValue(projectCategoryId, out var projectCategory))
                        {
                            project.Category = projectCategory;
                        }
                    }
                }
            } 
        }

        protected virtual async Task<IDictionary<decimal, IProjectCategory>> LoadProjectCategories(CancellationToken cancellationToken = default)
        {
            IDictionary<decimal, IProjectCategory> result = cache.ProjectCategories;
            if (result.Any() == false)
            {
                var projectCategoryService = projectCategoryServiceGetter();
                result = (await projectCategoryService.GetProjectCategoriesAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(projectCategory => projectCategory.Id);
            }
            return result;
        }
    }
}
