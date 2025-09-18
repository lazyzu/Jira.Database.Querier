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
    public class CustomFieldValueQuerySpecificationHandler : IIssueQuerySpecificationHandler
    {
        public IEnumerable<Type> HandleSchemaTarget { get; protected init; }

        public CustomFieldValueQuerySpecificationHandler()
        {
            HandleSchemaTarget = new Type[]
            {
                    typeof(customfieldvalue)
            };
        }

        public virtual IQuerySpecification Union(IQuerySpecification[] querySpecifications)
        {
            return QuerySpecificationExtension.Union<customfieldvalue>(querySpecifications);
        }

        public async Task<decimal[]> SearchIssueAsync(JiraContext jiraContext, IQuerySpecification querySpecification, decimal[] sourceIssueIds = null, CancellationToken cancellationToken = default)
        {
            if (querySpecification is QuerySpecification.QuerySpecification<customfieldvalue> customFieldQuerySpecification)
            {
                if (sourceIssueIds != null && sourceIssueIds.Length == 0) return new decimal[0];
                else
                {
                    var customFieldCriteria = await customFieldQuerySpecification.CriteriaGetter();

                    if (sourceIssueIds != null)
                    {
                        var nullableSourceIssueIds = sourceIssueIds.Cast<decimal?>();
                        customFieldCriteria = QuerySpecificationExtension.AndAlso(customfieldvalue => nullableSourceIssueIds.Contains(customfieldvalue.ISSUE), customFieldCriteria);
                    }

                    var issueQuery = jiraContext.customfieldvalue.AsNoTracking()
                        .Where(customFieldCriteria)
                        .Select(customfieldvalue => customfieldvalue.ISSUE);

                    return (await issueQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false))
                        .Select<decimal?, decimal>(issueId => issueId.Value)
                        .ToArray();
                }
            }
            else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported");
        }
    }
}
