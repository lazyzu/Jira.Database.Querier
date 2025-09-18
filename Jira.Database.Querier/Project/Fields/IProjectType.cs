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
    public class ProjectTypeProjection : IProjectProjectionSpecification<project>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<project, object>>[] IncludeExpressions { get; private init; }

        public ProjectTypeProjection()
        {
            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectType
            };

            IncludeExpressions = new Expression<Func<project, object>>[]
            {
                project => project.PROJECTTYPE
            };
        }

        public virtual Task Projection(project entity, JiraProject projection, CancellationToken cancellationToken = default)
        {
            projection.Type = entity.PROJECTTYPE;
            return Task.CompletedTask;
        }
    }

    internal class ProjectTypeSpecification : QuerySpecification<project>
    {
        public ProjectTypeSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((project project) => project.PROJECTTYPE, predicate));
        }
    }
}
