using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Services
{
    public interface IIssueStatusService
    {
        Task<IEnumerable<IIssueStatus>> GetStatusesAsync(CancellationToken cancellationToken = default);
        Task<IIssueStatus> GetStatusByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IIssueStatus> GetStatusByNameAsync(string name, CancellationToken cancellationToken = default);
    }

    public class IssueStatusService : IIssueStatusService
    {
        protected readonly JiraContext jiraContext;
        protected readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public IssueStatusService(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<IEnumerable<IIssueStatus>> GetStatusesAsync(CancellationToken cancellationToken = default)
        {
            if (cache.Statuses.Any() == false)
            {
                IDictionary<decimal, IIssueStatusCategory> statusCategories = cache.StatusCategories;
                if (statusCategories.Any() == false)
                {
                    var issueStatusCategoryService = jiraDatabaseQuerierGetter().Issue.IssueStatusCategory;
                    statusCategories = (await issueStatusCategoryService.GetStatusCategoriesAsync(cancellationToken).ConfigureAwait(false))
                        .ToDictionary(statusCategory => statusCategory.Id);
                }

                var query = jiraContext.issuestatus.AsNoTracking()
                    .OrderBy(dbModel => dbModel.SEQUENCE)
                    .Select(dbModel => new
                    {
                        Id = dbModel.ID,
                        Name = dbModel.pname,
                        Description = dbModel.DESCRIPTION,
                        Category = dbModel.STATUSCATEGORY
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var statusTempModel in queryResult)
                {
                    IIssueStatusCategory issueStatusCategory = default;
                    if (statusTempModel.Category.HasValue && statusCategories.TryGetValue(statusTempModel.Category.Value, out issueStatusCategory)) { }

                    cache.Statuses.TryAdd(statusTempModel.Id, new IssueStatus
                    {
                        Id = statusTempModel.Id,
                        Name = statusTempModel.Name,
                        Description = statusTempModel.Description,
                        Category = issueStatusCategory
                    });
                }
            }

            return cache.Statuses.Values;
        }

        public async Task<IIssueStatus> GetStatusByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            IDictionary<string, IIssueStatus> statuses = cache.Statuses;
            if (statuses.Any() == false)
            {
                statuses = (await this.GetStatusesAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(statusCategory => statusCategory.Id);
            }

            if (statuses.TryGetValue(id, out var status)) return status;
            else return default;
        }

        public async Task<IIssueStatus> GetStatusByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            foreach (var status in await GetStatusesAsync(cancellationToken).ConfigureAwait(false))
            {
                if ((status.Name ?? string.Empty).Equals(name)) return status;
            }

            return default;
        }
    }
}
