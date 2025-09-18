using lazyzu.Jira.Database.Querier.ProjectionSpecification;
using System.Collections.Generic;

namespace lazyzu.Jira.Database.Querier.User.Contract
{
    public interface IUserProjectionSpecification
    {
        IEnumerable<FieldKey> HandleTarget { get; }
    }

    public interface IUserProjectionWithContextSpecification<TEntity> : IUserProjectionSpecification, IProjectionWithContextSpecification<TEntity, JiraUser>
    { }

    public interface IUserProjectionSpecification<TEntity> : IUserProjectionSpecification, IProjectionSpecification<TEntity, JiraUser>
    { }

    public interface IUserExternalProjectionSpecification : IUserProjectionSpecification, IExternalProjectionSpecification<JiraUser>
    { }
}
