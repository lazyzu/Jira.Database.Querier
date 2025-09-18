using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Project.Fields;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueComponentProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public IssueComponentProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Components
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueComponentMap = await LoadIssueComponentMap(issueIds, jiraContext, cancellationToken).ConfigureAwait(false);

                if (issueComponentMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueComponentMap.TryGetValue(issue.Id, out var components)) issue.Components = components;
                        else issue.Components = new IProjectComponent[0];
                    }
                }
                else foreach (var issue in _issues)
                {
                    issue.Components = new IProjectComponent[0];
                }
            }
        }

        public virtual async Task<Dictionary<decimal, IProjectComponent[]>> LoadIssueComponentMap(decimal?[] issueIds, JiraContext jiraContext, CancellationToken cancellationToken = default)
        {
            var query = from nodeassociation in jiraContext.nodeassociation.AsNoTracking()
                        where issueIds.Contains(nodeassociation.SOURCE_NODE_ID)
                           && nodeassociation.SOURCE_NODE_ENTITY == "Issue"
                           && nodeassociation.ASSOCIATION_TYPE == "IssueComponent"
                        select new
                        {
                            nodeassociation.SOURCE_NODE_ID,
                            nodeassociation.SINK_NODE_ID
                        };

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var componentIds = queryResult.Select(dbModel => dbModel.SINK_NODE_ID).Distinct().ToArray();
            var componentMap = await ProjectComponentExtension.LoadComponentMap(componentIds, jiraContext, cancellationToken).ConfigureAwait(false);

            return queryResult.GroupBy(dbModel => dbModel.SOURCE_NODE_ID)
                .ToDictionary(issueIdGroup => issueIdGroup.Key
                            , issueIdGroup =>
                            {
                                var componentIds = issueIdGroup.Select(dbModel => dbModel.SINK_NODE_ID);
                                return LoadEntitiesFromId(componentIds, componentMap).ToArray();
                            });
        }

        protected virtual IEnumerable<TEntity> LoadEntitiesFromId<TEntity>(IEnumerable<decimal> ids, Dictionary<decimal, TEntity> map)
        {
            foreach (var id in ids)
            {
                if (map.TryGetValue(id, out var component)) yield return component;
            }
        }
    }
}
