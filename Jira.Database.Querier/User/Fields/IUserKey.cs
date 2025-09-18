using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.User.Contract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.User.Fields
{
    internal class UserKeyProjection : IUserProjectionSpecification<app_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<app_user, object>>[] IncludeExpressions { get; protected init; }

        public UserKeyProjection()
        {
            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserKey
            };

            IncludeExpressions = new Expression<Func<app_user, object>>[]
            {
                appUser => appUser.user_key,
            };
        }

        public Task Projection(app_user entity, JiraUser jiraUser, CancellationToken cancellationToken = default)
        {
            jiraUser.Key = entity.user_key;
            return Task.CompletedTask;
        }

    }
}
