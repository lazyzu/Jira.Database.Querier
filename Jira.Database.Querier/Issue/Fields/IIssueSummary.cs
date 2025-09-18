using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueSummaryProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; private init; }

        public IssueSummaryProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Summary
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.SUMMARY
            };
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            jiraIssue.Summary = entity.SUMMARY;
            return Task.CompletedTask;
        }
    }

    public class IssueSummarySpecification : QuerySpecification<jiraissue>
    {
        public IssueSummarySpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.SUMMARY, predicate));
        }
    }
}
