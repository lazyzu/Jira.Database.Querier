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
    public class IssueDescriptionProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        public IssueDescriptionProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Description
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.DESCRIPTION
            };
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            jiraIssue.Description = entity.DESCRIPTION;
            return Task.CompletedTask;
        }

    }

    public class IssueDescriptionSpecification : QuerySpecification<jiraissue>
    {
        public IssueDescriptionSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.DESCRIPTION, predicate));
        }
    }
}
