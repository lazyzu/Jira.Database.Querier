using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Project.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Fields.QuerySpecificationHandler
{
    public class ProjectQuerySpecificationHandler : IProjectQuerySpecificationHandler
    {
        public IEnumerable<Type> HandleSchemaTarget { get; protected init; }

        public ProjectQuerySpecificationHandler()
        {
            HandleSchemaTarget = new Type[]
            {
                typeof(project),
            };
        }

        public virtual IQuerySpecification Union(IQuerySpecification[] querySpecifications)
        {
            return QuerySpecificationExtension.Union<project>(querySpecifications);
        }

        public async Task<decimal[]> SearchIssueAsync(JiraContext jiraContext, IQuerySpecification querySpecification, decimal[] sourceProjectIds = null, CancellationToken cancellationToken = default)
        {
            if (querySpecification is QuerySpecification.QuerySpecification<project> jiraProjectQuerySpecification)
            {
                if (sourceProjectIds != null && sourceProjectIds.Length == 0) return new decimal[0];
                else
                {
                    var jiraProjectCriteria = await jiraProjectQuerySpecification.CriteriaGetter();

                    if (sourceProjectIds != null)
                    {
                        jiraProjectCriteria = QuerySpecificationExtension.AndAlso(jiraProject => sourceProjectIds.Contains(jiraProject.ID), jiraProjectCriteria);
                    }

                    var projectQuery = jiraContext.project.AsNoTracking()
                        .Where(jiraProjectCriteria)
                        .Select(project => project.ID);

                    return await projectQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported");
        }
    }
}
