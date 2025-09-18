using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public interface IIssueType
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        bool IsSubTask { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueType : IIssueType
    {
        [DefaultEquality]
        public string Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public bool IsSubTask { get; init; }

        public string pstyle { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Description})";
        }
    }

    public class IssueTypeProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        protected readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        protected readonly SharedCache cache;

        public IssueTypeProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache)
        {
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.IssueType
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.issuetype
            };
        }

        public virtual async Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            IDictionary<string, IIssueType> types = cache.IssueTypes;
            if (types.Any() == false)
            {
                var typeService = jiraDatabaseQuerierGetter().Issue.IssueType;
                types = (await typeService.GetIssueTypesAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(statusCategory => statusCategory.Id);
            }

            if (entity.issuetype != null && types.TryGetValue(entity.issuetype, out var issueType)) jiraIssue.IssueType = issueType;
        }
    }

    public class IssueTypeIdSpecification : QuerySpecification<jiraissue>
    {
        public IssueTypeIdSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.issuetype, predicate));
        }
    }
}
