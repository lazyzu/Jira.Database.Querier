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
    public interface IIssueChangelog
    {
        decimal Id { get; }
        string Author { get; }
        DateTime? Created { get; }
        IIssueChangelogItem[] Items { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueChangelog : IIssueChangelog
    {
        [DefaultEquality]
        public decimal Id { get; init; }

        public string Author { get; init; }

        public DateTime? Created { get; init; }

        public IIssueChangelogItem[] Items { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Author} {Created}";
        }
    }

    public interface IIssueChangelogItem
    {
        string Field { get; }
        string FieldType { get; }
        string OldValue { get; }
        string OldString { get; }
        string NewValue { get; }
        string NewString { get; }
    }

    public class IssueChangelogItem : IIssueChangelogItem
    {
        public string Field { get; init; }

        public string FieldType { get; init; }

        public string OldValue { get; init; }

        public string OldString { get; init; }

        public string NewValue { get; init; }

        public string NewString { get; init; }

        public override string ToString()
        {
            return $"{Field}: {OldString} -> {NewString}";
        }
    }

    public class IssueChangelogProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public IssueChangelogProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Changelogs
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueChangelogMap = await LoadIssueChangelogMap(issueIds, cancellationToken).ConfigureAwait(false);

                if (issueChangelogMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueChangelogMap.TryGetValue(issue.Id, out var changelogs)) issue.Changelogs = changelogs;
                        else issue.Changelogs = new IIssueChangelog[0];
                    }
                }
                else foreach (var issue in _issues)
                {
                    issue.Changelogs = new IIssueChangelog[0];
                }
            }
        }

        protected virtual async Task<Dictionary<decimal?, IssueChangelog[]>> LoadIssueChangelogMap(decimal?[] issueIds, CancellationToken cancellationToken)
        {

            var changeGroupQuery = from changegroup in jiraContext.changegroup.AsNoTracking()
                                   where issueIds.Contains(changegroup.issueid)
                                   select new
                                   {
                                       changegroup.ID,
                                       changegroup.issueid,
                                       changegroup.CREATED,
                                       changegroup.AUTHOR
                                   };

            var changeGroupQueryResult = await changeGroupQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var groupIds = changeGroupQueryResult.Select(dbModel => dbModel.ID as decimal?).ToArray();

            if (groupIds.Any())
            {
                var issueChangelogItemMap = await LoadIssueChangelogItem(groupIds, cancellationToken).ConfigureAwait(false);

                return changeGroupQueryResult.GroupBy(dbModel => dbModel.issueid)
                    .ToDictionary(issueIdGroup => issueIdGroup.Key
                                , issueIdGroup => issueIdGroup.Select(dbModel =>
                                {
                                    var changeItems = new IssueChangelogItem[0];
                                    if (issueChangelogItemMap.TryGetValue(dbModel.ID, out changeItems)) { }

                                    return new IssueChangelog
                                    {
                                        Id = dbModel.ID,
                                        Created = dbModel.CREATED,
                                        Author = dbModel.AUTHOR,
                                        Items = changeItems
                                    };
                                }).ToArray());


            }
            else return new Dictionary<decimal?, IssueChangelog[]>();

        }

        protected virtual async Task<Dictionary<decimal?, IssueChangelogItem[]>> LoadIssueChangelogItem(decimal?[] changeGroupIds, CancellationToken cancellationToken)
        {
            var query = jiraContext.changeitem.AsNoTracking()
                .Where(changeitem => changeGroupIds.Contains(changeitem.groupid))
                .Select(changeitem => new
                {
                    changeitem.groupid,
                    changeitem.FIELD,
                    changeitem.FIELDTYPE,
                    changeitem.OLDVALUE,
                    changeitem.OLDSTRING,
                    changeitem.NEWVALUE,
                    changeitem.NEWSTRING,
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.GroupBy(dbModel => dbModel.groupid)
                .ToDictionary(groupIdGrouped => groupIdGrouped.Key
                            , groupIdGrouped => groupIdGrouped.Select(dbModel => new IssueChangelogItem
                {
                    Field = dbModel.FIELD,
                    FieldType = dbModel.FIELDTYPE,
                    OldValue = dbModel.OLDVALUE,
                    OldString = dbModel.OLDSTRING,
                    NewValue = dbModel.NEWVALUE,
                    NewString = dbModel.NEWSTRING,
                }).ToArray());
        }
    }
}
