using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public interface IIssueStatus
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        IIssueStatusCategory Category { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueStatus : IIssueStatus
    {
        [DefaultEquality]
        public string Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public IIssueStatusCategory Category { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Description}, Category: {Category})";
        }
    }

    public class IssueStatusProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        protected readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        protected readonly SharedCache cache;

        public IssueStatusProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache)
        {
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.IssueStatus
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.issuestatus
            };
        }

        public virtual async Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            IDictionary<string, IIssueStatus> statuses = cache.Statuses;
            if (statuses.Any() == false)
            {
                var statusService = jiraDatabaseQuerierGetter().Issue.IssueStatus;
                statuses = (await statusService.GetStatusesAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(statusCategory => statusCategory.Id);
            }

            if (entity.issuestatus != null && statuses.TryGetValue(entity.issuestatus, out var issueStatus)) jiraIssue.IssueStatus = issueStatus;
        }
    }

    public class IssueStatusIdSpecification : QuerySpecification<jiraissue>
    {
        public IssueStatusIdSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.issuestatus, predicate));
        }
    }

    public class IssueStatusCategoryIdSpecification : QuerySpecification<jiraissue>
    {
        public IssueStatusCategoryIdSpecification(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, Func<decimal, bool> predicate)
        {
            CriteriaGetter = async () =>
            {
                var issueStatusService = jiraDatabaseQuerierGetter().Issue.IssueStatus;
                var matchedStatus = (await issueStatusService.GetStatusesAsync())
                .Where(status => predicate(status.Category.Id))
                .Select(status => status.Id)
                .ToArray();

                return QuerySpecificationExtension.Predict((jiraissue issue) => issue.issuestatus, statusId => matchedStatus.Contains(statusId));
            };
        }
    }
}
