using Generator.Equals;
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
    public interface IIssueComment
    {
        decimal Id { get; }
        string Author { get; }
        DateTime? Created { get; }

        string UpdateAuthor { get; }
        DateTime? Updated { get; }

        string Body { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueComment : IIssueComment
    {
        [DefaultEquality]
        public decimal Id { get; init; }

        public string Author { get; init; }

        public DateTime? Created { get; init; }

        public string UpdateAuthor { get; init; }

        public DateTime? Updated { get; init; }

        public string Body { get; init; }

        public override string ToString()
        {
            return Body;
        }
    }

    public class IssueCommentProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public IssueCommentProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Comments
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();
                var issueCommentMap = await LoadIssueCommentMap(issueIds, cancellationToken).ConfigureAwait(false);

                if (issueCommentMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueCommentMap.TryGetValue(issue.Id, out var comments)) issue.Comments = comments;
                        else issue.Comments = new IIssueComment[0];
                    }
                }
                else foreach (var issue in _issues)
                {
                    issue.Comments = new IIssueComment[0];
                }
            }
        }

        protected virtual async Task<Dictionary<decimal?, IssueComment[]>> LoadIssueCommentMap(decimal?[] issueIds, CancellationToken cancellationToken)
        {
            var query = jiraContext.jiraaction.AsNoTracking()
                                .Where(action => issueIds.Contains(action.issueid)
                                              && action.actiontype == "comment")
                                .Select(dbModel => new
                                {
                                    dbModel.ID,
                                    dbModel.issueid,
                                    dbModel.AUTHOR,
                                    dbModel.CREATED,
                                    dbModel.UPDATEAUTHOR,
                                    dbModel.UPDATED,
                                    dbModel.actionbody
                                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.GroupBy(dbModel => dbModel.issueid)
                .ToDictionary(issueIdGroup => issueIdGroup.Key
                            , issueIdGroup => issueIdGroup.Select(dbModel => new IssueComment
                            {
                                Id = dbModel.ID,
                                Author = dbModel.AUTHOR,
                                Created = dbModel.CREATED,
                                UpdateAuthor = dbModel.UPDATEAUTHOR,
                                Updated = dbModel.UPDATED,
                                Body = dbModel.actionbody
                            }).ToArray());
        }
    }
}
