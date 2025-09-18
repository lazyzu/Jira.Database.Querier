using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.User.Contract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.User.Fields
{
    public class UserAppIdProjection : IUserProjectionSpecification<app_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<app_user, object>>[] IncludeExpressions { get; protected init; }

        public UserAppIdProjection()
        {
            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserAppId
            };

            IncludeExpressions = new Expression<Func<app_user, object>>[]
            {
                appUser => appUser.ID
            };
        }

        public virtual Task Projection(app_user entity, JiraUser jiraUser, CancellationToken cancellationToken = default)
        {
            jiraUser.AppId = entity.ID;
            return Task.CompletedTask;
        }
    }
}
