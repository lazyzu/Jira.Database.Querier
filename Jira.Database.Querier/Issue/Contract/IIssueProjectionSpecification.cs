using lazyzu.Jira.Database.Querier.ProjectionSpecification;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Contract
{
    public interface IIssueProjectionSpecification
    {
        IEnumerable<FieldKey> HandleTarget { get; }
    }

    public interface IIssueProjectionSpecificationFieldKeyDependency
    {
        IIssueProjectionSpecification ConstructFrom(FieldKey fieldKey, FieldKey[] queryProjectionKeys);
    }

    public interface IIssueProjectionSpecification<TEntity> : IIssueProjectionSpecification, IProjectionSpecification<TEntity, JiraIssue>
    { }

    public interface IIssueProjectionWithContextSpecification<TEntity> : IIssueProjectionSpecification, IProjectionWithContextSpecification<TEntity, JiraIssue>
    { }

    public interface IIssueExternalProjectionSpecification : IIssueProjectionSpecification, IExternalProjectionSpecification<JiraIssue>
    { }

    public interface IIssueCustomFieldProjectionSpecification
    {
        bool IsSupported(ICustomFieldKey key);

        Task Projection(ICustomFieldKey key, IEnumerable<JiraIssue> projections, CancellationToken cancellationToken = default);
    }

    public interface IIssueCustomFieldProjectionSpecificationFieldKeyDependency
    {
        IIssueCustomFieldProjectionSpecification ConstructFrom(ICustomFieldKey fieldKey);
    }
}
