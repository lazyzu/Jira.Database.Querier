using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public interface IIssuePriority
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssuePriority : IIssuePriority
    {
        [DefaultEquality]
        public string Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Description})";
        }
    }

    public class IssuePriorityProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        private readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        private readonly SharedCache cache;

        public IssuePriorityProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache)
        {
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Priority
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.PRIORITY
            };
        }

        public virtual async Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            IDictionary<string, IIssuePriority> priorities = cache.Priorities;
            if (priorities.Any() == false)
            {
                var priorityService = jiraDatabaseQuerierGetter().Issue.IssuePriority;
                priorities = (await priorityService.GetPrioritiesAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(priority => priority.Id);
            }

            if (entity.PRIORITY != null && priorities.TryGetValue(entity.PRIORITY, out var priority)) jiraIssue.Priority = priority;
        }
    }

    public class IssuePriorityIdSpecification : QuerySpecification<jiraissue>
    {
        public IssuePriorityIdSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.PRIORITY, predicate));
        }
    }
}