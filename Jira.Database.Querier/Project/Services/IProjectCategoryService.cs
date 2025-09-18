using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Project.Fields;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Services
{
    public interface IProjectCategoryService
    {
        Task<IEnumerable<IProjectCategory>> GetProjectCategoriesAsync(CancellationToken cancellationToken = default);
    }

    public class ProjectCategoryService : IProjectCategoryService
    {
        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectCategoryService(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<IEnumerable<IProjectCategory>> GetProjectCategoriesAsync(CancellationToken cancellationToken = default)
        {
            if (cache.ProjectCategories.Any() == false)
            {
                var query = jiraContext.projectcategory.AsNoTracking()
                    .Select(dbModel => new ProjectCategory
                    {
                        Id = dbModel.ID,
                        Name = dbModel.cname,
                        Description = dbModel.description
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var projectCategory in queryResult)
                {
                    cache.ProjectCategories.TryAdd(projectCategory.Id, projectCategory);
                }
            }

            return cache.ProjectCategories.Values;
        }
    }
}
