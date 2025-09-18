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
    public class IssueResolutionDateProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        public IssueResolutionDateProjection()
        {
            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.ResolutionDate
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.RESOLUTIONDATE
            };
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            jiraIssue.ResolutionDate = entity.RESOLUTIONDATE;
            return Task.CompletedTask;
        }
    }

    public class IssueResolutionDateSpecification : QuerySpecification<jiraissue>
    {
        public IssueResolutionDateSpecification(Expression<Func<DateTime?, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.RESOLUTIONDATE, predicate));
        }
    }
}
