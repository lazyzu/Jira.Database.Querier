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
    public interface IIssueResolution
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueResolution : IIssueResolution
    {
        [DefaultEquality]
        public string Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Description})";
        }
    }

    public class IssueResolutionProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        private readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        private readonly SharedCache cache;

        public IssueResolutionProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache)
        {
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Resolution
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.RESOLUTION
            };
        }

        public virtual async Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            IDictionary<string, IIssueResolution> resolutions = cache.Resolutions;
            if (resolutions.Any() == false)
            {
                var resolutionService = jiraDatabaseQuerierGetter().Issue.IssueResolution;
                resolutions = (await resolutionService.GetResolutionsAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(resolution => resolution.Id);
            }

            if (entity.RESOLUTION != null && resolutions.TryGetValue(entity.RESOLUTION, out var resolution)) jiraIssue.Resolution = resolution;
        }
    }

    public class IssueResolutionIdSpecification : QuerySpecification<jiraissue>
    {
        public IssueResolutionIdSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.RESOLUTION, predicate));
        }
    }
}
