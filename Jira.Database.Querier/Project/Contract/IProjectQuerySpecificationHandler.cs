using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Contract
{
    public interface IProjectQuerySpecificationHandler
    {
        IEnumerable<Type> HandleSchemaTarget { get; }
        IQuerySpecification Union(IQuerySpecification[] querySpecifications);
        Task<decimal[]> SearchIssueAsync(JiraContext jiraContext, IQuerySpecification querySpecification, decimal[] sourceProjectIds = null, CancellationToken cancellationToken = default);
    }
}
