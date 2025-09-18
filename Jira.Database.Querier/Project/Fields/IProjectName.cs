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
    public class ProjectNameProjection : IProjectProjectionSpecification<project>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<project, object>>[] IncludeExpressions { get; private init; }

        public ProjectNameProjection()
        {
            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectName
            };

            IncludeExpressions = new Expression<Func<project, object>>[]
            {
                project => project.pname
            };
        }

        public virtual Task Projection(project entity, JiraProject projection, CancellationToken cancellationToken = default)
        {
            projection.Name = entity.pname;
            return Task.CompletedTask;
        }

    }

    internal class ProjectNameSpecification : QuerySpecification<project>
    {
        public ProjectNameSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((project project) => project.pname, predicate));
        }
    }
}
