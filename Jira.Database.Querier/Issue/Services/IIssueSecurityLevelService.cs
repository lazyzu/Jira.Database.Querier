using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Services
{
    public interface IIssueSecurityLevelService
    {
        Task<IEnumerable<IIssueSecurityLevelScheme>> GetSecurityLevelSchemasAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<IIssueSecurityLevel>> GetSecurityLevelsAsync(bool foreceReload = false,CancellationToken cancellationToken = default);

        Task<Dictionary<decimal, IIssueSecurityLevel[]>> GetSecurityLevelsAsync(IEnumerable<decimal> schemeIds, bool foreceReload = false, CancellationToken cancellationToken = default);
    }

    public class IssueSecurityLevelService : IIssueSecurityLevelService
    {
        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public IssueSecurityLevelService(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<IEnumerable<IIssueSecurityLevelScheme>> GetSecurityLevelSchemasAsync(CancellationToken cancellationToken = default)
        {
            if (cache.SecurityLevelSchemes.Any() == false)
            {
                var securityLevelSchemeQuery = jiraContext.issuesecurityscheme.AsNoTracking();
                var securityLevelSchemeQueryResult = await securityLevelSchemeQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                var defaultSecurityLevelId = securityLevelSchemeQueryResult
                    .Select(dbModel => dbModel.DEFAULTLEVEL)
                    .Where(securityLevelId => securityLevelId.HasValue)
                    .ToArray();

                var securityLevelQuery = jiraContext.schemeissuesecuritylevels.AsNoTracking()
                    .Where(dbModel => defaultSecurityLevelId.Contains(dbModel.ID));
                var securityLevelQueryResult = await securityLevelQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                var securityLevelSchemeResults =
                    from securityLevelScheme in securityLevelSchemeQueryResult
                    from securityLevel in securityLevelQueryResult.Where(_securityLevel => _securityLevel.ID == securityLevelScheme.DEFAULTLEVEL).DefaultIfEmpty()
                    select new IssueSecurityLevelScheme(id: securityLevelScheme.ID
                    , name: securityLevelScheme.NAME
                    , description: securityLevelScheme.DESCRIPTION
                    , defaultValue: securityLevel == null ? null : (id: securityLevel.ID, name: securityLevel.NAME, description: securityLevel.DESCRIPTION));

                foreach (var securityLevelScheme in securityLevelSchemeResults)
                {
                    cache.SecurityLevelSchemes.TryAdd(securityLevelScheme.Id, securityLevelScheme);
                }
            }

            return cache.SecurityLevelSchemes.Values;
        }

        public async Task<IEnumerable<IIssueSecurityLevel>> GetSecurityLevelsAsync(bool foreceReload = false, CancellationToken cancellationToken = default)
        {
            if (foreceReload || cache.SecurityLevels.Any() == false)
            {
                IDictionary<decimal, IIssueSecurityLevelScheme> securityLevelSchemeMap = cache.SecurityLevelSchemes;
                if (securityLevelSchemeMap.Any() == false)
                {
                    securityLevelSchemeMap = (await GetSecurityLevelSchemasAsync(cancellationToken).ConfigureAwait(false))
                        .ToDictionary(securityLevelSchema => securityLevelSchema.Id);
                }

                var query = jiraContext.schemeissuesecuritylevels.AsNoTracking()
                    .Select(dbModel => new
                    {
                        dbModel.ID,
                        dbModel.NAME,
                        dbModel.DESCRIPTION,
                        dbModel.SCHEME
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var dbModel in queryResult)
                {
                    IIssueSecurityLevelScheme matchedIssueSecurityLevelScheme = null;
                    if (dbModel.SCHEME.HasValue) securityLevelSchemeMap.TryGetValue(dbModel.SCHEME.Value, out matchedIssueSecurityLevelScheme);

                    var outputIssueSecurityLevel = new IssueSecurityLevel
                    {
                        Id = dbModel.ID,
                        Name = dbModel.NAME,
                        Description = dbModel.DESCRIPTION,
                        Scheme = matchedIssueSecurityLevelScheme
                    };

                    if (cache.SecurityLevels.TryAdd(outputIssueSecurityLevel.Id, outputIssueSecurityLevel))
                    {
                        cache.SecurityLevels[outputIssueSecurityLevel.Id] = outputIssueSecurityLevel;
                    }
                }
            }

            return cache.SecurityLevels.Values;
        }

        public async Task<Dictionary<decimal, IIssueSecurityLevel[]>> GetSecurityLevelsAsync(IEnumerable<decimal> schemeIds, bool foreceReload = false, CancellationToken cancellationToken = default)
        {
            var _schemeIds = schemeIds?.Select(schemeId => (decimal?)schemeId)?.ToArray() ?? new decimal?[0];
            if (_schemeIds.Length == 0) return new Dictionary<decimal, IIssueSecurityLevel[]>();

            var schemeLoaded = cache.SecurityLevels.Any(securityLevel => _schemeIds.Contains(securityLevel.Value.Scheme.Id));

            if (foreceReload || !schemeLoaded)
            {
                IDictionary<decimal, IIssueSecurityLevelScheme> securityLevelSchemeMap = cache.SecurityLevelSchemes;
                if (securityLevelSchemeMap.Any() == false)
                {
                    securityLevelSchemeMap = (await GetSecurityLevelSchemasAsync(cancellationToken).ConfigureAwait(false))
                        .ToDictionary(securityLevelSchema => securityLevelSchema.Id);
                }

                var query = jiraContext.schemeissuesecuritylevels.AsNoTracking()
                    .Where(dbModel => _schemeIds.Contains(dbModel.SCHEME))
                    .Select(dbModel => new
                    {
                        dbModel.ID,
                        dbModel.NAME,
                        dbModel.DESCRIPTION,
                        dbModel.SCHEME
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var dbModel in queryResult)
                {
                    IIssueSecurityLevelScheme matchedIssueSecurityLevelScheme = null;
                    if (dbModel.SCHEME.HasValue) securityLevelSchemeMap.TryGetValue(dbModel.SCHEME.Value, out matchedIssueSecurityLevelScheme);

                    var outputIssueSecurityLevel = new IssueSecurityLevel
                    {
                        Id = dbModel.ID,
                        Name = dbModel.NAME,
                        Description = dbModel.DESCRIPTION,
                        Scheme = matchedIssueSecurityLevelScheme
                    };

                    if (cache.SecurityLevels.TryAdd(outputIssueSecurityLevel.Id, outputIssueSecurityLevel) == false)
                    {
                        cache.SecurityLevels[outputIssueSecurityLevel.Id] = outputIssueSecurityLevel;
                    }
                }
            }

            return cache.SecurityLevels.Values.Where(securityLevel => _schemeIds.Contains(securityLevel.Scheme.Id))
                .GroupBy(securityLevel => securityLevel.Scheme.Id).ToDictionary(schemeGroup => schemeGroup.Key, schemeGroup => schemeGroup.ToArray());
        }
    }
}
