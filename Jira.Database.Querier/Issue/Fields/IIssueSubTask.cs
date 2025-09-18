using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueSubTaskProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        protected readonly IIssueLinkType jiraSubtaskLinkType;
        protected readonly ILogger logger;

        public IssueSubTaskProjection(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, IIssueLinkType jiraSubtaskLinkType, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.jiraSubtaskLinkType = jiraSubtaskLinkType;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.SubTaskIds
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal>(issue => issue.Id).ToArray();

                var issueLinkService = jiraDatabaseQuerierGetter().Issue.IssueLink;
                var issueLinkMap = await issueLinkService.GetLinksForIssueAsync(issueIds, LinkDirectionEnum.Outward, new IIssueLinkType[]
                {
                    jiraSubtaskLinkType
                }, cancellationToken).ConfigureAwait(false);

                foreach (var issue in _issues)
                {
                    if (issueLinkMap.TryGetValue(issue.Id, out var links))
                    {
                        issue.SubTaskIds = links.Select(link => link.InwardIssueId.Value).ToArray();
                    }
                    else issue.SubTaskIds = new decimal[0];
                }
            }
        }
    }
}
