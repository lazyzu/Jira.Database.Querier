using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueKeyProjection : IIssueProjectionWithContextSpecification<jiraissue>, IIssueProjectionSpecificationFieldKeyDependency
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }


        protected readonly IssueProjectProjection issueProjectProjection;
        protected readonly IssueNumProjection issueNumProjection;

        protected readonly Func<IProjectService> projectServiceGetter;

        public IssueKeyProjection(Func<IProjectService> projectServiceGetter, FieldKey[] queryProjectionKeys = null)
        {
            this.projectServiceGetter = projectServiceGetter;

            var projectQueried = queryProjectionKeys?.Any(field => IssueFieldSelection.Project.Equals(field)) ?? false;
            var issueNumQueried = queryProjectionKeys?.Any(field => IssueFieldSelection.IssueNum.Equals(field)) ?? false;

            if (projectQueried == false) this.issueProjectProjection = new IssueProjectProjection(projectServiceGetter);
            if (issueNumQueried == false) this.issueNumProjection = new IssueNumProjection();

            this.HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Key
            };

            this.IncludeExpressions = buildIncludeExpressions(issueProjectProjection, issueNumProjection).ToArray();
        }

        public IssueKeyProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter) : this(() => jiraDatabaseQuerierGetter().Project)
        { }

        protected IEnumerable<Expression<Func<jiraissue, object>>> buildIncludeExpressions(IssueProjectProjection issueProjectProjection, IssueNumProjection issueNumProjection)
        {
            if (issueProjectProjection != null)
            {
                foreach (var includeExpression in issueProjectProjection.IncludeExpressions) yield return includeExpression;
            }

            if (issueNumProjection != null)
            {
                foreach (var includeExpression in issueNumProjection.IncludeExpressions) yield return includeExpression;
            }
        }

        public IIssueProjectionSpecification ConstructFrom(FieldKey fieldKey, FieldKey[] queryProjectionKeys)
        {
            return new IssueKeyProjection(projectServiceGetter, queryProjectionKeys);
        }

        public async Task<object> PrepareContext(IEnumerable<jiraissue> enties, CancellationToken cancellationToken = default)
        {
            if (issueProjectProjection != null) return await this.issueProjectProjection.PrepareContext(enties, cancellationToken).ConfigureAwait(false);
            else return null;
        }

        public async Task Projection(jiraissue entity, JiraIssue projection, object context, CancellationToken cancellationToken = default)
        {
            if (issueProjectProjection != null) await this.issueProjectProjection.Projection(entity, projection, context, cancellationToken).ConfigureAwait(false);
            if (issueNumProjection != null) await this.issueNumProjection.Projection(entity, projection, cancellationToken).ConfigureAwait(false);
        }
    }
}
