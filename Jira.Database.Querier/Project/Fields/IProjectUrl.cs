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
    public class ProjectUrlProjection : IProjectProjectionSpecification<project>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<project, object>>[] IncludeExpressions { get; private init; }

        public ProjectUrlProjection()
        {
            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectUrl
            };

            IncludeExpressions = new Expression<Func<project, object>>[]
            {
                project => project.URL
            };
        }

        public virtual Task Projection(project entity, JiraProject projection, CancellationToken cancellationToken = default)
        {
            projection.Url = entity.URL;
            return Task.CompletedTask;
        }
    }

    internal class ProjectUrlSpecification : QuerySpecification<project>
    {
        public ProjectUrlSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((project project) => project.URL, predicate));
        }
    }
}
