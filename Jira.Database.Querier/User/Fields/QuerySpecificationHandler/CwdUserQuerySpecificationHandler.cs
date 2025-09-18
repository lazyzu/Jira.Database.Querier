using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using lazyzu.Jira.Database.Querier.User.Contract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.User.Fields.QuerySpecificationHandler
{
    public class CwdUserQuerySpecificationHandler : IUserQuerySpecificationHandler
    {
        public IEnumerable<Type> HandleSchemaTarget { get; protected init; }

        public CwdUserQuerySpecificationHandler()
        {
            HandleSchemaTarget = new Type[]
            {
                typeof(cwd_user)
            };
        }

        public IQuerySpecification Union(IQuerySpecification[] querySpecifications)
        {
            return QuerySpecificationExtension.Union<cwd_user>(querySpecifications);
        }

        public async Task<string[]> SearchIssueAsync(JiraContext jiraContext, IQuerySpecification querySpecification, string[] sourceUserNames = null, CancellationToken cancellationToken = default)
        {
            if (querySpecification is QuerySpecification<cwd_user> cwdUserQuerySpec)
            {
                if(sourceUserNames != null && sourceUserNames.Length == 0) return new string[0];

                var userCriteria = await cwdUserQuerySpec.CriteriaGetter();

                if (sourceUserNames != null)
                {
                    userCriteria = QuerySpecificationExtension.AndAlso(cwdUser => sourceUserNames.Contains(cwdUser.lower_user_name), userCriteria);
                }

                var cwdUserQuery = jiraContext.cwd_user.AsNoTracking()
                    .Where(userCriteria)
                    .Select(cwdUser => cwdUser.lower_user_name);

                return await cwdUserQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);
            }
            else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported user spec");
        }
    }
}
