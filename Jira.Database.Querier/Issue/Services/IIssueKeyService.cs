using lazyzu.Jira.Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Services
{
    public interface IIssueKeyService
    {
        Task<Dictionary<string, decimal>> GetIssueIdAsync(IEnumerable<string> issueKeys, CancellationToken cancellationToken = default);
        Task<decimal?> GetIssueIdAsync(string issueKey, CancellationToken cancellationToken = default);
        Task<bool?> IsMovedAsync(string issueKey, CancellationToken cancellationToken = default);
    }

    public class IssueKeyService : IIssueKeyService
    {
        protected readonly JiraContext jiraContext;
        protected readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public IssueKeyService(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;
            this.logger = logger;
        }

        public virtual async Task<decimal?> GetIssueIdAsync(string issueKey, CancellationToken cancellationToken = default)
        {
            var result = await GetIssueIdAsync(new string[] { issueKey }, cancellationToken);
            if (result.Any()) return result.First().Value;
            else return null;
        }

        public virtual async Task<Dictionary<string, decimal>> GetIssueIdAsync(IEnumerable<string> issueKeys, CancellationToken cancellationToken = default)
        {
            var validIssueKeys = validIssueKeyCheck(issueKeys);
            await RefreshIssueIdCache(validIssueKeys, argumentCheck: false, cancellationToken).ConfigureAwait(false);
            await RefreshIssueIdCacheFromMoveRecord(validIssueKeys, argumentCheck: false, cancellationToken).ConfigureAwait(false);

            return (from issueKeyRecord in cache.IssueKeyRecords
                    from targetIssueKey in validIssueKeys.Where(_targetIssueKey => issueKeyRecord.Key == _targetIssueKey)
                    select issueKeyRecord)
                    .ToDictionary(map => map.Key
                                , map => map.Value.IssueId);
        }

        public virtual async Task<bool?> IsMovedAsync(string issueKey, CancellationToken cancellationToken = default)
        {
            var _issueKey = issueKey?.Trim();
            if (string.IsNullOrEmpty(_issueKey)) return null;
            else
            {
                await RefreshIssueIdCacheFromMoveRecord(new string[] { _issueKey }, argumentCheck: false, cancellationToken).ConfigureAwait(false);

                if (cache.IssueKeyRecords.TryGetValue(issueKey, out var record))
                {
                    return record.Moved;
                }
                else return null;
            }
        }

        protected virtual async Task RefreshIssueIdCache(IEnumerable<string> issueKeys, bool argumentCheck = false, CancellationToken cancellationToken = default)
        {
            var _issueKeys = argumentCheck ? validIssueKeyCheck(issueKeys) : issueKeys?.ToArray() ?? new string[0];

            if (_issueKeys.Any() == false) return;

            var projectKeyService = jiraDatabaseQuerierGetter().Project.ProjectKey;

            // Build cache with un-exist item
            var issueKeyNeedToQuery = _issueKeys.Except(cache.IssueKeyRecords.Keys).ToArray();
            if (issueKeyNeedToQuery.Any())
            {
                foreach (var projectKeyGroup in ToProjectIssueNumStruc(issueKeyNeedToQuery).GroupBy(projectIssueNumStruc => projectIssueNumStruc.ProjectKey))
                {
                    var targetProjectKey = projectKeyGroup.Key;
                    var targetProjectId = await projectKeyService.GetProjectIdAsync(targetProjectKey, cancellationToken).ConfigureAwait(false);
                    if (targetProjectId.HasValue == false) continue;

                    var targetIssueNums = projectKeyGroup.Select(i => (decimal?)i.IssueNum).ToArray();

                    var query = from issue in jiraContext.jiraissue.AsNoTracking()
                                where issue.PROJECT == targetProjectId
                                   && targetIssueNums.Contains(issue.issuenum)
                                select new
                                {
                                    IssueId = issue.ID,
                                    IssueNum = issue.issuenum
                                };

                    var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                    foreach (var issueKeyRecord in queryResult)
                    {
                        cache.IssueKeyRecords.TryAdd($"{targetProjectKey}-{issueKeyRecord.IssueNum}", new SharedCache.IssueIdMapRecord
                        {
                            IssueId = issueKeyRecord.IssueId,
                            Moved = false
                        });
                    }
                }
            }
        }

        protected IEnumerable<(string ProjectKey, decimal IssueNum)> ToProjectIssueNumStruc(string[] issueKeys)
        {
            if (issueKeys != null && issueKeys.Length != 0)
            {
                foreach (var issueKey in issueKeys)
                {
                    var parts = issueKey.Split("-");
                    if (parts.Length == 2)
                    {
                        if (decimal.TryParse(parts[1], out var issueNum))
                        {
                            var projectKey = parts[0];
                            yield return (projectKey, issueNum);
                        }
                    }
                }
            }
        }

        protected virtual async Task RefreshIssueIdCacheFromMoveRecord(IEnumerable<string> issueKeys, bool argumentCheck = false, CancellationToken cancellationToken = default)
        {
            var _issueKeys = argumentCheck ? validIssueKeyCheck(issueKeys) : issueKeys?.ToArray() ?? new string[0];

            if (_issueKeys.Any() == false) return;

            // Build cache with un-exist item
            var issueKeyNeedToQuery = _issueKeys.Except(cache.IssueKeyRecords.Keys).ToArray();
            if (issueKeyNeedToQuery.Any())
            {
                var query = jiraContext.moved_issue_key.AsNoTracking()
                .Where(movedIssueKey => issueKeyNeedToQuery.Contains(movedIssueKey.OLD_ISSUE_KEY))
                .Select(movedIssueKey => new
                {
                    movedIssueKey.OLD_ISSUE_KEY,
                    movedIssueKey.ISSUE_ID
                });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var movedIssueKeyRecord in queryResult)
                {
                    if (movedIssueKeyRecord.ISSUE_ID.HasValue)
                    {
                        cache.IssueKeyRecords.TryAdd(movedIssueKeyRecord.OLD_ISSUE_KEY, new SharedCache.IssueIdMapRecord
                        {
                            IssueId = movedIssueKeyRecord.ISSUE_ID.Value,
                            Moved = true
                        });
                    }
                }
            }
        }

        protected static string[] validIssueKeyCheck(IEnumerable<string> issueKeys)
        {
            return issueKeys?.Select(issueKey => issueKey?.Trim())
                ?.Where(issueKey => string.IsNullOrEmpty(issueKey) == false)
                ?.ToArray() ?? new string[0];
        }
    }
}
