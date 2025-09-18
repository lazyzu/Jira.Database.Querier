using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueProjectProjection : IIssueProjectionWithContextSpecification<jiraissue>, IIssueProjectionSpecificationFieldKeyDependency
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        protected readonly Func<IProjectService> projectServiceGetter;

        public Project.Contract.FieldKey[] ProjectKeys { get; protected init; } = null;

        public IssueProjectProjection(Func<IProjectService> projectServiceGetter)
        {
            this.projectServiceGetter = projectServiceGetter;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Project
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.PROJECT
            };
        }

        public IssueProjectProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter) : this(() => jiraDatabaseQuerierGetter().Project) 
        { }

        public IIssueProjectionSpecification ConstructFrom(FieldKey fieldKey, FieldKey[] queryProjectionKeys)
        {
            if (fieldKey is Project.Contract.IProjectFieldKeyCollection projectFieldKeyCollection) return new IssueProjectProjection(this.projectServiceGetter)
            {
                ProjectKeys = projectFieldKeyCollection.Fields
            };
            else return this;
        }

        public virtual async Task<object> PrepareContext(IEnumerable<jiraissue> enties, CancellationToken cancellationToken = default)
        {
            var _enties = enties?.ToArray() ?? new jiraissue[0];

            if (_enties.Length == 0) return new Dictionary<decimal, Project.IJiraProject>();

            var projectIds = _enties.Where(issue => issue.PROJECT.HasValue)
                .Select(issue => issue.PROJECT.Value)
                .Distinct()
                .ToArray();

            var projectService = projectServiceGetter();
            var projects = await projectService.GetProjectsAsync(projectIds, fields: ProjectKeys ?? projectService.DefaultQueryFields.ToArray(), cancellationToken);

            return projects.ToDictionary(project => project.Id);
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, object context, CancellationToken cancellationToken = default)
        {
            var map = context as IDictionary<decimal, IJiraProject>;
            if (entity.PROJECT.HasValue && map.TryGetValue(entity.PROJECT.Value, out var project)) jiraIssue.Project = project;
            return Task.CompletedTask;
        }
    }

    public class IssueProjectIdSpecification : QuerySpecification<jiraissue>
    {
        public IssueProjectIdSpecification(Expression<Func<decimal?, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.PROJECT, predicate));
        }
    }
}
