using lazyzu.Jira.Database.Querier.QuerySpecification;
using lazyzu.Jira.Database.Querier.User.Contract;
using lazyzu.Jira.Database.Querier.User.Fields;
using System;
using System.Linq.Expressions;

namespace lazyzu.Jira.Database.Querier.User.Contract
{
    public interface IUserSpecs
    {
        IQuerySpecification Name(Expression<Func<string, bool>> predicate);
        IQuerySpecification DisplayName(Expression<Func<string, bool>> predicate);
        IQuerySpecification Email(Expression<Func<string, bool>> predicate);
        IQuerySpecification Active(Expression<Func<decimal?, bool>> predicate);

        // TODO: Try to support key!!
    }
}

namespace lazyzu.Jira.Database.Querier
{
    public static class UserSpecExtension
    {
        public static IQuerySpecification Active(this IUserSpecs specs, bool isActive)
        {
            if (isActive) return specs.Active(isActiveNum => isActiveNum > 0);
            else return specs.Active(isActiveNum => isActiveNum == 0);
        }
    }
}

namespace lazyzu.Jira.Database.Querier.User
{
    public class UserSpecs : IUserSpecs
    {
        public IQuerySpecification Name(Expression<Func<string, bool>> predicate)
            => new UsernameSpecification(predicate);

        public IQuerySpecification DisplayName(Expression<Func<string, bool>> predicate)
            => new UserDisplayNameSpecification(predicate);

        public IQuerySpecification Email(Expression<Func<string, bool>> predicate)
            => new UserEmailSpecification(predicate);

        public IQuerySpecification Active(Expression<Func<decimal?, bool>> predicate)
            => new UserActiveSpecification(predicate);
    }
}
