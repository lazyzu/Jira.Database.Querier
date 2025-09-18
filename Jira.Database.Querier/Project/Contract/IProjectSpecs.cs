using lazyzu.Jira.Database.Querier.Project.Contract;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Linq.Expressions;

namespace lazyzu.Jira.Database.Querier.Project.Contract
{
    public interface IProjectSpecs
    {
        IQuerySpecification Id(Expression<Func<decimal, bool>> predicate);
        IQuerySpecification Name(Expression<Func<string, bool>> predicate);
        IQuerySpecification Url(Expression<Func<string, bool>> predicate);
        IQuerySpecification Lead(Expression<Func<string, bool>> predicate);
        IQuerySpecification Description(Expression<Func<string, bool>> predicate);
        IQuerySpecification Key(Expression<Func<string, bool>> predicate);
        IQuerySpecification Type(Expression<Func<string, bool>> predicate);
    }
}

namespace lazyzu.Jira.Database.Querier
{
    public static class ProjectSpecExtension
    { }
}

namespace lazyzu.Jira.Database.Querier.Project
{
    public class ProjectSpecs : IProjectSpecs
    {
        public ProjectSpecs()
        { }

        public IQuerySpecification Id(Expression<Func<decimal, bool>> predicate)
            => new ProjectIdSpecification(predicate);

        public IQuerySpecification Name(Expression<Func<string, bool>> predicate)
            => new ProjectNameSpecification(predicate);

        public IQuerySpecification Url(Expression<Func<string, bool>> predicate)
            => new ProjectUrlSpecification(predicate);

        public IQuerySpecification Lead(Expression<Func<string, bool>> predicate)
            => new ProjectLeadSpecification(predicate);

        public IQuerySpecification Description(Expression<Func<string, bool>> predicate)
            => new ProjectDescriptionSpecification(predicate);

        public IQuerySpecification Key(Expression<Func<string, bool>> predicate)
            => new ProjectKeySpecification(predicate);

        public IQuerySpecification Type(Expression<Func<string, bool>> predicate)
             => new ProjectTypeSpecification(predicate);
    }
}
