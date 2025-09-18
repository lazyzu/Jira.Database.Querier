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
    public class NodeAssociationQuerySpecificationHandler : IIssueQuerySpecificationHandler
    {
        public IEnumerable<Type> HandleSchemaTarget { get; protected init; }

        public NodeAssociationQuerySpecificationHandler()
        {
            HandleSchemaTarget = new Type[]
            {
                    typeof(nodeassociation)
            };
        }

        public virtual IQuerySpecification Union(IQuerySpecification[] querySpecifications)
        {
            return QuerySpecificationExtension.Union<nodeassociation>(querySpecifications);
        }

        public async Task<decimal[]> SearchIssueAsync(JiraContext jiraContext, IQuerySpecification querySpecification, decimal[] sourceIssueIds = null, CancellationToken cancellationToken = default)
        {
            if (querySpecification is QuerySpecification.QuerySpecification<nodeassociation> nodeAssociationQuerySpecification)
            {
                if (sourceIssueIds != null && sourceIssueIds.Length == 0) return new decimal[0];
                else
                {
                    var nodeAssociationCriteria = await nodeAssociationQuerySpecification.CriteriaGetter();

                    if (sourceIssueIds != null)
                    {
                        nodeAssociationCriteria = QuerySpecificationExtension.AndAlso(nodeassociation => sourceIssueIds.Contains(nodeassociation.SOURCE_NODE_ID), nodeAssociationCriteria);
                    }

                    var issueQuery = jiraContext.nodeassociation.AsNoTracking()
                        .Where(nodeAssociationCriteria)
                        .Select(nodeassociation => nodeassociation.SOURCE_NODE_ID);

                    return await issueQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported");
        }
    }
}
