using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Services
{
    public interface IIssueResolutionService
    {
        Task<IEnumerable<IIssueResolution>> GetResolutionsAsync(CancellationToken cancellationToken = default);
    }

    public class IssueResolutionService : IIssueResolutionService
    {
        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public IssueResolutionService(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;
        }

        public virtual async Task<IEnumerable<IIssueResolution>> GetResolutionsAsync(CancellationToken cancellationToken = default)
        {
            if (cache.Resolutions.Any() == false)
            {
                var query = jiraContext.resolution.AsNoTracking()
                    .OrderBy(dbModel => dbModel.SEQUENCE)
                    .Select<resolution, IIssueResolution>(dbModel => new IssueResolution
                    {
                        Id = dbModel.ID,
                        Name = dbModel.pname,
                        Description = dbModel.DESCRIPTION
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var resolution in queryResult)
                {
                    cache.Resolutions.TryAdd(resolution.Id, resolution);
                }
            }

            return cache.Resolutions.Values;
        }
    }
}
