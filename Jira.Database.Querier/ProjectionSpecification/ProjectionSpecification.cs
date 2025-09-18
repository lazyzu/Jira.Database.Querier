using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.ProjectionSpecification
{
    public interface IProjectionSpecification
    { }

    public interface IProjectionIncludeSpecification<TEntity>
    {
        Expression<Func<TEntity, object>>[] IncludeExpressions { get; }
    }

    public interface IProjectionWithContextSpecification<TEntity, TProjection> : IProjectionSpecification, IProjectionIncludeSpecification<TEntity>
    {
        Task<object> PrepareContext(IEnumerable<TEntity> enties, CancellationToken cancellationToken = default);
        Task Projection(TEntity entity, TProjection projection, object context, CancellationToken cancellationToken = default);
    }

    public interface IProjectionSpecification<TEntity, TProjection> : IProjectionSpecification, IProjectionIncludeSpecification<TEntity>
    {
        Task Projection(TEntity entity, TProjection projection, CancellationToken cancellationToken = default);
    }

    public class CompositionProjectionSpecification : IProjectionSpecification
    {
        public readonly IProjectionSpecification[] ProjectionSpecifications;
        public CompositionProjectionSpecification(params IProjectionSpecification[] projectionSpecifications)
        {
            ProjectionSpecifications = projectionSpecifications;
        }
    }

    public static class ProjectionIncludeSpecificationExtension
    {
        public static IQueryable<TEntity> ProjectionInclude<TEntity>(this IQueryable<TEntity> inputQuery, IEnumerable<IProjectionIncludeSpecification<TEntity>> projectionIncludeSpecifications, string[] additionalFields = null)
        {
            var projectionExpressions = projectionIncludeSpecifications?.SelectMany(spec => spec.IncludeExpressions);
            return inputQuery.ProjectionInclude(projectionExpressions, additionalFields);
        }

        public static IQueryable<TEntity> ProjectionInclude<TEntity>(this IQueryable<TEntity> inputQuery, IEnumerable<Expression<Func<TEntity, object>>> projectionExpressions, string[] additionalFields = null)
        {
            var tempRequiredFieldNames = projectionExpressions?.LoadProjectionFieldNames();

            if (additionalFields != null) tempRequiredFieldNames = tempRequiredFieldNames?.Concat(additionalFields);

            var requiredFieldNames = tempRequiredFieldNames
                ?.Distinct()
                ?.ToArray() ?? new string[0];

            return inputQuery.SelectMembers(requiredFieldNames);
        }

        public static IEnumerable<string> LoadProjectionFieldNames<TEntity>(this IEnumerable<Expression<Func<TEntity, object>>> projectionExpressions)
        {
            if (projectionExpressions != null)
            {
                foreach (var projectionExpression in projectionExpressions)
                {
                    if (projectionExpression.Body is System.Linq.Expressions.UnaryExpression unaryExpression)
                    {
                        if (unaryExpression.Operand.NodeType == ExpressionType.MemberAccess
                         && unaryExpression.Operand is System.Linq.Expressions.MemberExpression memberExpression)
                        {
                            yield return memberExpression.Member.Name;
                        }
                    }
                    else if (projectionExpression.Body is System.Linq.Expressions.MemberExpression memberExpression)
                    {
                        yield return memberExpression.Member.Name;
                    }
                }
            }
        }

        public static IQueryable<TEntity> SelectMembers<TEntity>(this IQueryable<TEntity> inputQuery, params string[] memberNames)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var bindings = memberNames
                .Select(name => Expression.PropertyOrField(parameter, name))
                .Select(member => Expression.Bind(member.Member, member));
            var body = Expression.MemberInit(Expression.New(typeof(TEntity)), bindings);
            var selector = Expression.Lambda<Func<TEntity, TEntity>>(body, parameter);
            return inputQuery.Select(selector);
        }
    }

    public interface IExternalProjectionSpecification<TProjection> : IProjectionSpecification
    {
        Task Projection(IEnumerable<TProjection> projections, CancellationToken cancellationToken = default);
    }
}
