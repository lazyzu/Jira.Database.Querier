using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.User.Contract
{
    public interface IUserQuerySpecificationHandler
    {
        IEnumerable<Type> HandleSchemaTarget { get; }
        IQuerySpecification Union(IQuerySpecification[] querySpecifications);
        Task<string[]> SearchIssueAsync(JiraContext jiraContext, IQuerySpecification querySpecification, string[] sourceUserNames = null, CancellationToken cancellationToken = default);

    }
}
