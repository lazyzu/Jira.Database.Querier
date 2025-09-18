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
    public interface IIssuePriorityService
    {
        Task<IEnumerable<IIssuePriority>> GetPrioritiesAsync(CancellationToken cancellationToken = default);
    }

    public class IssuePriorityService : IIssuePriorityService
    {
        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public IssuePriorityService(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;
        }

        public virtual async Task<IEnumerable<IIssuePriority>> GetPrioritiesAsync(CancellationToken cancellationToken = default)
        {
            if (cache.Priorities.Any() == false)
            {
                var query = jiraContext.priority.AsNoTracking()
                    .OrderBy(dbModel => dbModel.SEQUENCE)
                    .Select<priority, IIssuePriority>(dbModel => new IssuePriority
                    {
                        Id = dbModel.ID,
                        Name = dbModel.pname,
                        Description = dbModel.DESCRIPTION
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var priority in queryResult)
                {
                    cache.Priorities.TryAdd(priority.Id, priority);
                }
            }

            return cache.Priorities.Values;
        }
    }
}
