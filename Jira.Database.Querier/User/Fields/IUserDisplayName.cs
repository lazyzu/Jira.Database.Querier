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
    public class UserDisplayNameProjection : IUserProjectionSpecification<cwd_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<cwd_user, object>>[] IncludeExpressions { get; protected set; }

        public UserDisplayNameProjection()
        {
            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserDisplayName
            };

            IncludeExpressions = new Expression<Func<cwd_user, object>>[]
            {
                cwdUser => cwdUser.display_name
            };
        }

        public virtual Task Projection(cwd_user cwdUser, JiraUser jiraUser, CancellationToken cancellationToken = default)
        {
            jiraUser.DisplayName = cwdUser.display_name;
            return Task.CompletedTask;
        }
    }

    internal class UserDisplayNameSpecification : QuerySpecification<cwd_user>
    {
        public UserDisplayNameSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((cwd_user cwdUser) => cwdUser.display_name, predicate));
        }
    }
}
