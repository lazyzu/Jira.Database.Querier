using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Services
{
    public interface IIssueStatusCategoryService
    {
        Task<IEnumerable<IIssueStatusCategory>> GetStatusCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IIssueStatusCategory> GetStatusCategoryAsync(decimal id, CancellationToken cancellationToken = default);
        Task<IIssueStatusCategory> GetStatusCategoryAsync(string name, CancellationToken cancellationToken = default);
    }

    public class IssueStatusCategoryService : IIssueStatusCategoryService
    {
        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public IssueStatusCategoryService(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;
        }

        public Task<IEnumerable<IIssueStatusCategory>> GetStatusCategoriesAsync(CancellationToken cancellationToken = default)
        {
            if (cache.StatusCategories.Any() == false)
            {
                cache.StatusCategories.TryAdd(1, new IssueStatusCategory
                {
                    Id = 1,
                    Name = "No Category"
                });
                cache.StatusCategories.TryAdd(2, new IssueStatusCategory
                {
                    Id = 2,
                    Name = "To Do"
                });
                cache.StatusCategories.TryAdd(4, new IssueStatusCategory
                {
                    Id = 4,
                    Name = "In Progress"
                });
                cache.StatusCategories.TryAdd(3, new IssueStatusCategory
                {
                    Id = 3,
                    Name = "Done"
                });
            }

            return Task.FromResult<IEnumerable<IIssueStatusCategory>>(cache.StatusCategories.Values);
        }

        public async Task<IIssueStatusCategory> GetStatusCategoryAsync(decimal id, CancellationToken cancellationToken = default)
        {
            IDictionary<decimal, IIssueStatusCategory> statusCategories = cache.StatusCategories;
            if (statusCategories.Any() == false)
            {
                statusCategories = (await this.GetStatusCategoriesAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(statusCategory => statusCategory.Id);
            }

            if (statusCategories.TryGetValue(id, out var statusCategory)) return statusCategory;
            else return default;
        }

        public async Task<IIssueStatusCategory> GetStatusCategoryAsync(string name, CancellationToken cancellationToken = default)
        {
            foreach (var statusCategory in await GetStatusCategoriesAsync(cancellationToken).ConfigureAwait(false))
            {
                if ((statusCategory.Name ?? string.Empty).Equals(name)) return statusCategory;
            }
            return default;
        }
    }
}
