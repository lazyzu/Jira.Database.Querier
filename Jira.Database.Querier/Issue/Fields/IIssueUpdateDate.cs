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
    public class IssueUpdateDateProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        public IssueUpdateDateProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.UpdateDate
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.UPDATED
            };
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            jiraIssue.UpdateDate = entity.UPDATED;
            return Task.CompletedTask;
        }

    }

    public class IssueUpdateDateSpecification : QuerySpecification<jiraissue>
    {
        public IssueUpdateDateSpecification(Expression<Func<DateTime?, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.UPDATED, predicate));
        }
    }
}
