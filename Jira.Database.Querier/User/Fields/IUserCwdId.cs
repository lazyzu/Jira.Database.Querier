using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.User.Contract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.User.Fields
{
    public class UserCwdIdProjection : IUserProjectionSpecification<cwd_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<cwd_user, object>>[] IncludeExpressions { get; protected set; }

        public UserCwdIdProjection()
        {
            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserCwdId
            };

            IncludeExpressions = new Expression<Func<cwd_user, object>>[]
            {
                cwdUser => cwdUser.ID
            };
        }

        public virtual Task Projection(cwd_user cwdUser, JiraUser jiraUser, CancellationToken cancellationToken = default)
        {
            jiraUser.CwdId = cwdUser.ID;
            return Task.CompletedTask;
        }
    }
}
