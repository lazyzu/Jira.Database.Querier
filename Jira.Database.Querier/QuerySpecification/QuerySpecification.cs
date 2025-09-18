using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.QuerySpecification
{
    public interface IQuerySpecification
    {
        Type SchemaType { get; }
    }

    public abstract class QuerySpecification<TEntity> : IQuerySpecification
    {
        public Type SchemaType => typeof(TEntity);

        public Func<Task<Expression<Func<TEntity, bool>>>> CriteriaGetter { get; protected init; }

        internal class And : QuerySpecification<TEntity>
        {
            public And(QuerySpecification<TEntity> left, QuerySpecification<TEntity> right)
            {
                CriteriaGetter = async () =>
                {
                    var leftCriteria = await left.CriteriaGetter();
                    var rightCriteria = await right.CriteriaGetter();

                    var parameters = leftCriteria.Parameters;
                    var andExpression = Expression.AndAlso(leftCriteria.Body, rightCriteria.Body.ReplaceParameter(rightCriteria.Parameters[0], parameters[0]));

                    return Expression.Lambda<Func<TEntity, bool>>(andExpression, parameters);
                };
            }
        }

        internal class Or : QuerySpecification<TEntity>
        {
            public Or(QuerySpecification<TEntity> left, QuerySpecification<TEntity> right)
            {
                CriteriaGetter = async () =>
                {
                    var leftCriteria = await left.CriteriaGetter();
                    var rightCriteria = await right.CriteriaGetter();

                    var parameters = leftCriteria.Parameters;
                    var orExpression = Expression.OrElse(leftCriteria.Body, rightCriteria.Body.ReplaceParameter(rightCriteria.Parameters[0], parameters[0]));
                    
                    return Expression.Lambda<Func<TEntity, bool>>(orExpression, parameters);
                };
            }
        }

        internal class Not : QuerySpecification<TEntity>
        {
            public Not(QuerySpecification<TEntity> spec)
            {
                CriteriaGetter = async () =>
                {
                    var specCriteria = await spec.CriteriaGetter();
                    var notExpression = Expression.Not(specCriteria.Body);
                    var parameters = new ParameterExpression[]
                    {
                        Expression.Parameter(typeof(TEntity), specCriteria.Parameters.First().Name)
                    };
                    return Expression.Lambda<Func<TEntity, bool>>(notExpression, parameters);
                };
            }
        }

        public static QuerySpecification<TEntity> operator &(QuerySpecification<TEntity> left, QuerySpecification<TEntity> right)
        {
            return new And(left, right);
        }

        public static QuerySpecification<TEntity> operator |(QuerySpecification<TEntity> left, QuerySpecification<TEntity> right)
        {
            return new Or(left, right);
        }

        public static QuerySpecification<TEntity> operator !(QuerySpecification<TEntity> spec)
        {
            return new Not(spec);
        }
    }

    public static class QuerySpecificationExtension
    {
        //public static QuerySpecification<TEntity> And<TEntity>(this QuerySpecification<TEntity> left, QuerySpecification<TEntity> right)
        //    => new QuerySpecification<TEntity>.And(left, right);

        //public static QuerySpecification<TEntity> Or<TEntity>(this QuerySpecification<TEntity> left, QuerySpecification<TEntity> right)
        //    => new QuerySpecification<TEntity>.Or(left, right);

        //public static QuerySpecification<TEntity> Not<TEntity>(this QuerySpecification<TEntity> spec)
        //    => new QuerySpecification<TEntity>.Not(spec);

        public static Expression<Func<TEntity, bool>> Predict<TEntity, TEntityProp>(Expression<Func<TEntity, TEntityProp>> prop, Expression<Func<TEntityProp, bool>> predicate
            , Expression<Func<TEntity, bool>> baseCriteria = null)
        {
            var tempResult = prop.Predict(predicate);

            if (baseCriteria == null) return tempResult;
            else
            {
                var parameters = baseCriteria.Parameters;
                var andExpression = Expression.AndAlso(baseCriteria.Body, tempResult.Body.ReplaceParameter(tempResult.Parameters[0], parameters[0]));

                return Expression.Lambda<Func<TEntity, bool>>(andExpression, parameters);
            }
        }

        public static Expression<Func<TEntity, TResult>> Predict<TEntity, TEntityProp, TResult>(
            this Expression<Func<TEntity, TEntityProp>> propertyExpression,
            Expression<Func<TEntityProp, TResult>> predictExpression)
        {
            var param = Expression.Parameter(typeof(TEntity));
            var intermediateValue = propertyExpression.Body.ReplaceParameter(propertyExpression.Parameters[0], param);
            var body = predictExpression.Body.ReplaceParameter(predictExpression.Parameters[0], intermediateValue);
            return Expression.Lambda<Func<TEntity, TResult>>(body, param);
        }

        public static Expression ReplaceParameter(this Expression expression,
            ParameterExpression toReplace,
            Expression newExpression)
        {
            return new ParameterReplaceVisitor(toReplace, newExpression)
                .Visit(expression);
        }

        public static Expression<Func<TEntity, bool>> AndAlso<TEntity>(Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right)
        {
            var parameters = left.Parameters;
            var andExpression = Expression.AndAlso(left.Body, right.Body.ReplaceParameter(right.Parameters[0], parameters[0]));
            
            return Expression.Lambda<Func<TEntity, bool>>(andExpression, parameters);
        }

        public static IQuerySpecification Union<TEntity>(IQuerySpecification[] querySpecifications)
        {
            if (querySpecifications == null) return null;
            else
            {
                QuerySpecification<TEntity> result = null;
                foreach (var querySpecification in querySpecifications)
                {
                    if (querySpecification is QuerySpecification<TEntity> jiraIssueQuerySpecification)
                    {
                        if (result == null) result = jiraIssueQuerySpecification;
                        else result = result & jiraIssueQuerySpecification;
                    }
                }
                return result;
            }
        }

        private class ParameterReplaceVisitor : ExpressionVisitor
        {
            private ParameterExpression from;
            private Expression to;
            public ParameterReplaceVisitor(ParameterExpression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == from ? to : node;
            }
        }
    }
}
