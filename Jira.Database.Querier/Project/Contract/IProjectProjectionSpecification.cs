using lazyzu.Jira.Database.Querier.ProjectionSpecification;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace lazyzu.Jira.Database.Querier.Project.Contract
{
    public interface IProjectProjectionSpecification
    {
        IEnumerable<FieldKey> HandleTarget { get; }
    }

    public interface IProjectProjectionSpecificationFieldKeyDependency
    {
        IProjectProjectionSpecification ConstructFrom(FieldKey fieldKey, ImmutableArray<FieldKey> queryProjectionKeys);
    }

    public interface IProjectProjectionWithContextSpecification<TEntity> : IProjectProjectionSpecification, IProjectionWithContextSpecification<TEntity, JiraProject>
    { }

    public interface IProjectProjectionSpecification<TEntity> : IProjectProjectionSpecification, IProjectionSpecification<TEntity, JiraProject>
    { }

    public interface IProjectExternalProjectionSpecification : IProjectProjectionSpecification, IExternalProjectionSpecification<JiraProject>
    { }
}
