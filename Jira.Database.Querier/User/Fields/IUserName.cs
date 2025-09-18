using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using lazyzu.Jira.Database.Querier.User.Contract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.User.Fields
{
    internal class UserNameProjection : IUserProjectionSpecification<cwd_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<cwd_user, object>>[] IncludeExpressions { get; protected set; }

        public UserNameProjection()
        {
            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserName
            };

            IncludeExpressions = new Expression<Func<cwd_user, object>>[]
            {
                cwdUser => cwdUser.user_name
            };
        }

        public Task Projection(cwd_user cwdUser, JiraUser jiraUser, CancellationToken cancellationToken = default)
        {
            jiraUser.Username = cwdUser.user_name;
            return Task.CompletedTask;
        }
    }

    internal class UsernameSpecification : QuerySpecification<cwd_user>
    {
        public UsernameSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((cwd_user cwdUser) => cwdUser.user_name, predicate));
        }
    }
}
