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
    public class UserActiveProjection : IUserProjectionSpecification<cwd_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<cwd_user, object>>[] IncludeExpressions { get; protected set; }

        public UserActiveProjection()
        {
            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserActive
            };

            IncludeExpressions = new Expression<Func<cwd_user, object>>[]
            {
                cwdUser => cwdUser.active
            };
        }

        public virtual Task Projection(cwd_user cwdUser, JiraUser jiraUser, CancellationToken cancellationToken = default)
        {
            jiraUser.IsActive = cwdUser.active.HasValue && cwdUser.active > 0;
            return Task.CompletedTask;
        }
    }

    internal class UserActiveSpecification : QuerySpecification<cwd_user>
    {
        public UserActiveSpecification(Expression<Func<decimal?, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((cwd_user cwdUser) => cwdUser.active, predicate));
        }
    }
}
