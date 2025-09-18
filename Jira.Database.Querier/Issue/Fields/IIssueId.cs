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
    internal class IssueIdProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        public IssueIdProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.IssueId
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                project => project.ID
            };
        }

        public virtual Task Projection(jiraissue entity, JiraIssue projection, CancellationToken cancellationToken = default)
        {
            projection.Id = entity.ID;
            return Task.CompletedTask;
        }
    }

    internal class IssueIdSpecification : QuerySpecification<jiraissue>
    {
        public IssueIdSpecification(Expression<Func<decimal, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.ID, predicate));
        }
    }
}
