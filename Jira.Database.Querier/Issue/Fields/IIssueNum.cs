using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueNumProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        public IssueNumProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.IssueNum
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.issuenum
            };
        }

        public Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            jiraIssue.IssueNum = entity.issuenum;
            return Task.CompletedTask;
        }
    }
}
