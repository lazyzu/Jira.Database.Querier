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
    public class ProjectKeyProjection : IProjectProjectionSpecification<project>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<project, object>>[] IncludeExpressions { get; private init; }

        public ProjectKeyProjection()
        {
            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectKey
            };

            IncludeExpressions = new Expression<Func<project, object>>[]
            {
                project => project.pkey
            };
        }

        public virtual Task Projection(project entity, JiraProject projection, CancellationToken cancellationToken = default)
        {
            projection.Key = entity.pkey;
            return Task.CompletedTask;
        }
    }

    internal class ProjectKeySpecification : QuerySpecification<project>
    {
        public ProjectKeySpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((project project) => project.pkey, predicate));
        }
    }
}
