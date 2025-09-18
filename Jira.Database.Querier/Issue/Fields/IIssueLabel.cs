using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueLabelProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public IssueLabelProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Labels
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueLabelMap = await BuildIssueLabelMap(issueIds, cancellationToken).ConfigureAwait(false);

                if (issueLabelMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueLabelMap.TryGetValue(issue.Id, out var labels)) issue.Labels = labels;
                        else issue.Labels = new string[0];
                    }
                }
                else foreach (var issue in _issues)
                {
                    issue.Labels = new string[0];
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, string[]>> BuildIssueLabelMap(decimal?[] issueIds, CancellationToken cancellationToken = default)
        {
            if (issueIds.Any())
            {
                var query = jiraContext.label.AsNoTracking()
                    .Where(label => issueIds.Contains(label.ISSUE)
                                 && label.FIELDID == null)
                    .Select(label => new
                    {
                        label.ISSUE,
                        label.LABEL1
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                return queryResult.GroupBy(dbModel => dbModel.ISSUE.Value)
                    .ToDictionary(issueIdGroup => issueIdGroup.Key
                                , issueIdGroup => issueIdGroup.Select(dbModel => dbModel.LABEL1).ToArray());
            }
            else return new Dictionary<decimal, string[]>();
        }
    }
}
