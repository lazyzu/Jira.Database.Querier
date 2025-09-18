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
    public class UserEmailProjection : IUserProjectionSpecification<cwd_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<cwd_user, object>>[] IncludeExpressions { get; protected set; }

        public UserEmailProjection()
        {
            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserEmail
            };

            IncludeExpressions = new Expression<Func<cwd_user, object>>[]
            {
                cwdUser => cwdUser.email_address
            };
        }

        public virtual Task Projection(cwd_user cwdUser, JiraUser jiraUser, CancellationToken cancellationToken = default)
        {
            jiraUser.Email = cwdUser.email_address;
            return Task.CompletedTask;
        }
    }

    internal class UserEmailSpecification : QuerySpecification<cwd_user>
    {
        public UserEmailSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((cwd_user cwdUser) => cwdUser.email_address, predicate));
        }
    }
}
