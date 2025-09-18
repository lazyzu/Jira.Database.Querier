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
    public class IssueEnvironmentProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        public IssueEnvironmentProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Environment
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.ENVIRONMENT
            };
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            jiraIssue.Environment = entity.ENVIRONMENT;
            return Task.CompletedTask;
        }
    }

    public class IssueEnvironmentSpecification : QuerySpecification<jiraissue>
    {
        public IssueEnvironmentSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.REPORTER, predicate));
        }
    }
}
