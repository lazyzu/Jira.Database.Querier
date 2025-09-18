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
    public class IssueCreateDateProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        public IssueCreateDateProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.CreateDate
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.CREATED
            };
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            jiraIssue.CreateDate = entity.CREATED;
            return Task.CompletedTask;
        }

    }

    public class IssueCreateDateSpecification : QuerySpecification<jiraissue>
    {
        public IssueCreateDateSpecification(Expression<Func<DateTime?, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.CREATED, predicate));
        }
    }
}
