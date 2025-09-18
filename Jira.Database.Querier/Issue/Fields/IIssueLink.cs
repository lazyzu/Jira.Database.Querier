using Generator.Equals;
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
    public interface IIssueLink
    {
        public decimal Id { get; }

        public IIssueLinkType LinkType { get; }

        public decimal? InwardIssueId { get; }

        public decimal? OutwardIssueId { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueLink : IIssueLink
    {
        [DefaultEquality]
        public decimal Id { get; init; }

        public IIssueLinkType LinkType { get; init; }

        public decimal? InwardIssueId { get; init; }

        public decimal? OutwardIssueId { get; init; }

        public override string ToString()
        {
            return $"{Id}: {InwardIssueId} -{LinkType.Outward}-> {OutwardIssueId}";
        }
    }

    public interface IIssueLinkType
    {
        decimal Id { get; }
        string Name { get; }
        string Inward { get; }
        string Outward { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueLinkType : IIssueLinkType
    {
        [DefaultEquality]
        public decimal Id { get; init; }   // API Model is string
        public string Name { get; init; }
        public string Inward { get; init; }
        public string Outward { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name} (Inward:{Inward}, Outward:{Outward})";
        }
    }

    [Flags]
    public enum LinkDirectionEnum
    {
        None,
        Inward,
        Outward,
        All = Inward | Outward
    }

    public class IssueLinkProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        protected readonly ILogger logger;

        public IssueLinkProjection(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.IssueLinks
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal>(issue => issue.Id).ToArray();

                var issueLinkService = jiraDatabaseQuerierGetter().Issue.IssueLink;
                var issueLinkMap = await issueLinkService.GetLinksForIssueAsync(issueIds, LinkDirectionEnum.All, cancellationToken: cancellationToken).ConfigureAwait(false);

                foreach (var issue in _issues)
                {
                    if (issueLinkMap.TryGetValue(issue.Id, out var links))
                    {
                        issue.IssueLinks = links.ToArray();
                    }
                    else issue.IssueLinks = new IIssueLink[0];
                }
            }
        }
    }
}
