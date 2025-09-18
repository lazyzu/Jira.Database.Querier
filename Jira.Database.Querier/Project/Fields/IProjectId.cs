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
    internal interface IProjectIdProjection : IProjectProjectionSpecification<project> { }

    internal class ProjectIdProjection : IProjectIdProjection
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<project, object>>[] IncludeExpressions { get; private init; }

        public ProjectIdProjection()
        {
            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectId
            };

            IncludeExpressions = new Expression<Func<project, object>>[]
            {
                project => project.ID
            };
        }

        public Task Projection(project entity, JiraProject projection, CancellationToken cancellationToken = default)
        {
            projection.Id = entity.ID;
            return Task.CompletedTask;
        }
    }

    internal class ProjectIdSpecification : QuerySpecification<project>
    {
        public ProjectIdSpecification(Expression<Func<decimal, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((project project) => project.ID, predicate));
        }
    }
}
