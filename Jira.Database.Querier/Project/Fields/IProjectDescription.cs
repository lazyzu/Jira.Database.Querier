using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Project.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Fields
{
    public class ProjectDescriptionProjection : IProjectProjectionSpecification<project>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<project, object>>[] IncludeExpressions { get; private init; }

        public ProjectDescriptionProjection()
        {
            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectDescription
            };

            IncludeExpressions = new Expression<Func<project, object>>[]
            {
                project => project.DESCRIPTION
            };
        }

        public virtual Task Projection(project entity, JiraProject projection, CancellationToken cancellationToken = default)
        {
            projection.Description = entity.DESCRIPTION;
            return Task.CompletedTask;
        }
    }

    internal class ProjectDescriptionSpecification : QuerySpecification<project>
    {
        public ProjectDescriptionSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((project project) => project.DESCRIPTION, predicate));
        }
    }
}
