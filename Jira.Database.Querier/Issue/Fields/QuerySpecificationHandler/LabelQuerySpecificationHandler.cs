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
    public class LabelQuerySpecificationHandler : IIssueQuerySpecificationHandler
    {
        public IEnumerable<Type> HandleSchemaTarget { get; protected init; }

        public LabelQuerySpecificationHandler()
        {
            HandleSchemaTarget = new Type[]
            {
                    typeof(label)
            };
        }

        public virtual IQuerySpecification Union(IQuerySpecification[] querySpecifications)
        {
            return QuerySpecificationExtension.Union<label>(querySpecifications);
        }

        public async Task<decimal[]> SearchIssueAsync(JiraContext jiraContext, IQuerySpecification querySpecification, decimal[] sourceIssueIds = null, CancellationToken cancellationToken = default)
        {
            if (querySpecification is QuerySpecification.QuerySpecification<label> labelQuerySpecification)
            {
                if (sourceIssueIds != null && sourceIssueIds.Length == 0) return new decimal[0];
                else
                {
                    var labelCriteria = await labelQuerySpecification.CriteriaGetter();

                    if (sourceIssueIds != null)
                    {
                        var nullableSourceIssueIds = sourceIssueIds.Cast<decimal?>();
                        labelCriteria = QuerySpecificationExtension.AndAlso(label => nullableSourceIssueIds.Contains(label.ISSUE), labelCriteria);
                    }

                    var issueQuery = jiraContext.label.AsNoTracking()
                        .Where(labelCriteria)
                        .Select(label => label.ISSUE)
                        .Distinct();

                    return (await issueQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false))
                        .Select<decimal?, decimal>(issueId => issueId.Value)
                        .ToArray();
                }
            }
            else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported");
        }
    }
}
