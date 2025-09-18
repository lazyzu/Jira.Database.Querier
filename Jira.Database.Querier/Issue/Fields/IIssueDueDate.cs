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
    public class IssueDueDateProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        public IssueDueDateProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.DueDate
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.DUEDATE
            };
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            jiraIssue.DueDate = entity.DUEDATE;
            return Task.CompletedTask;
        }
    }

    public class IssueDueDateSpecification : QuerySpecification<jiraissue>
    {
        public IssueDueDateSpecification(Expression<Func<DateTime?, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.DUEDATE, predicate));
        }
    }
}
