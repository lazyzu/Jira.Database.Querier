using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields.QuerySpecificationHandler
{
    public class JiraIssueQuerySpecificationHandler : IIssueQuerySpecificationHandler
    {
        public IEnumerable<Type> HandleSchemaTarget { get; protected init; }

        public JiraIssueQuerySpecificationHandler()
        {
            HandleSchemaTarget = new Type[]
            {
                typeof(jiraissue)
            };
        }

        public virtual IQuerySpecification Union(IQuerySpecification[] querySpecifications)
        {
            return QuerySpecificationExtension.Union<jiraissue>(querySpecifications);
        }

        public virtual async Task<decimal[]> SearchIssueAsync(JiraContext jiraContext, IQuerySpecification querySpecification, decimal[] sourceIssueIds = null, CancellationToken cancellationToken = default)
        {
            if (querySpecification is QuerySpecification.QuerySpecification<jiraissue> jiraIssueQuerySpecification)
            {
                if (sourceIssueIds != null && sourceIssueIds.Length == 0) return new decimal[0];
                else
                {
                    var jiraIssueCriteria = await jiraIssueQuerySpecification.CriteriaGetter();

                    if (sourceIssueIds != null)
                    {
                        jiraIssueCriteria = QuerySpecificationExtension.AndAlso(jiraissue => sourceIssueIds.Contains(jiraissue.ID), jiraIssueCriteria);
                    }

                    var issueQuery = jiraContext.jiraissue.AsNoTracking()
                        .Where(jiraIssueCriteria)
                        .Select(issue => issue.ID);

                    return await issueQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported");
        }
    }
}
