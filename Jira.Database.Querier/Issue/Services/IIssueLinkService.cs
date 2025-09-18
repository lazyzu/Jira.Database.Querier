using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
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
    public interface IIssueLinkService
    {
        Task<IEnumerable<IIssueLinkType>> GetLinkTypesAsync(CancellationToken cancellationToken = default);
        Task<IIssueLink[]> GetLinksForIssueAsync(decimal issueId, LinkDirectionEnum linkDirection = LinkDirectionEnum.All, IEnumerable<IIssueLinkType> linkTypes = null, CancellationToken cancellationToken = default);
        Task<Dictionary<decimal, IIssueLink[]>> GetLinksForIssueAsync(IEnumerable<decimal> issueIds, LinkDirectionEnum linkDirection = LinkDirectionEnum.All, IEnumerable<IIssueLinkType> linkTypes = null, CancellationToken cancellationToken = default);
    }

    public class IssueLinkService : IIssueLinkService
    {
        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public IssueLinkService(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;
        }

        public virtual async Task<IEnumerable<IIssueLinkType>> GetLinkTypesAsync(CancellationToken cancellationToken = default)
        {
            if (cache.LinkTypes.Any() == false)
            {
                var query = jiraContext.issuelinktype.AsNoTracking().Select(dbModel => new IssueLinkType
                {
                    Id = dbModel.ID,
                    Name = dbModel.LINKNAME,
                    Inward = dbModel.INWARD,
                    Outward = dbModel.OUTWARD
                } as IIssueLinkType);

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var issueLinkType in queryResult)
                {
                    cache.LinkTypes.TryAdd(issueLinkType.Id, issueLinkType);
                }
            }

            return cache.LinkTypes.Values;
        }

        public virtual async Task<IIssueLink[]> GetLinksForIssueAsync(decimal issueId
            , LinkDirectionEnum linkDirection = LinkDirectionEnum.All
            , IEnumerable<IIssueLinkType> linkTypes = null
            , CancellationToken cancellationToken = default)
        {
            var results = await GetLinksForIssueAsync(new decimal[] { issueId }, linkDirection, linkTypes, cancellationToken).ConfigureAwait(false);

            if (results.TryGetValue(issueId, out var result)) return result;
            else return new IIssueLink[0];
        }

        public virtual async Task<Dictionary<decimal, IIssueLink[]>> GetLinksForIssueAsync(IEnumerable<decimal> issueIds
            , LinkDirectionEnum linkDirection = LinkDirectionEnum.All
            , IEnumerable<IIssueLinkType> linkTypes = null
            , CancellationToken cancellationToken = default)
        {
            var _issueIds = issueIds?.Select<decimal, decimal?>(issueId => issueId)
                                    ?.Distinct()
                                    ?.ToArray() ?? new decimal?[0];

            if (_issueIds.Length == 0) return new Dictionary<decimal, IIssueLink[]>();

            var query = jiraContext.issuelink.AsNoTracking();

            if (linkTypes != null)
            {
                query = filterByLinkType(query, linkTypes.ToArray());
            }

            query = filterByDirection(query, _issueIds, linkDirection);

            var queryResults = await executeQuery(query, cancellationToken).ConfigureAwait(false);

            return (from issueId in _issueIds
                    from issueLink in queryResults.Where(matchIssueInDirection(issueId, linkDirection))
                    group issueLink by issueId)
                    .ToDictionary(issueIdGroup => issueIdGroup.Key.Value
                                , issueIdGroup => issueIdGroup.ToArray());
        }

        protected virtual IQueryable<issuelink> filterByLinkType(IQueryable<issuelink> query, IIssueLinkType[] linkTypes)
        {
            if (linkTypes == null || linkTypes.Length == 0) return query.Where(link => false);

            var targetLinkTypeIds = linkTypes.Select<IIssueLinkType, decimal?>(linkType => linkType.Id).ToArray();

            return query.Where(link => targetLinkTypeIds.Contains(link.LINKTYPE));
        }

        protected virtual IQueryable<issuelink> filterByDirection(IQueryable<issuelink> query, decimal?[] issueIds, LinkDirectionEnum linkDirection)
        {
            if (issueIds == null || issueIds.Length == 0) return query.Where(link => false);

            if (linkDirection == LinkDirectionEnum.None) return query.Where(link => false);
            if (linkDirection == LinkDirectionEnum.All)
            {
                return query.Where(link => issueIds.Contains(link.SOURCE) || issueIds.Contains(link.DESTINATION));
            }
            else if (linkDirection == LinkDirectionEnum.Inward)
            {
                return query.Where(link => issueIds.Contains(link.DESTINATION));
            }
            else if (linkDirection == LinkDirectionEnum.Outward)
            {
                return query.Where(link => issueIds.Contains(link.SOURCE));
            }
            else return query.Where(link => false);
        }

        protected virtual async Task<IIssueLink[]> executeQuery(IQueryable<issuelink> query, CancellationToken cancellationToken)
        {
            var queryResult = await query.Select(link => new
            {
                link.ID,
                link.LINKTYPE,
                link.SOURCE,
                link.DESTINATION
            }).ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var issueLinkTypeMap = (await GetLinkTypesAsync(cancellationToken).ConfigureAwait(false)).ToDictionary(linkType => linkType.Id);

            return queryResult.Select(link =>
            {
                IIssueLinkType linkType = null;
                if (link.LINKTYPE.HasValue)
                {
                    if (issueLinkTypeMap.TryGetValue(link.LINKTYPE.Value, out linkType)) { }
                }

                return new IssueLink
                {
                    Id = link.ID,
                    LinkType = linkType,
                    InwardIssueId = link.DESTINATION,
                    OutwardIssueId = link.SOURCE,
                };
            }).ToArray();
        }

        protected static Func<IIssueLink, bool> matchIssueInDirection(decimal? issueId, LinkDirectionEnum linkDirection)
        {
            if (linkDirection == LinkDirectionEnum.Inward) return _issueLink => _issueLink.InwardIssueId == issueId;
            if (linkDirection == LinkDirectionEnum.Outward) return _issueLink => _issueLink.OutwardIssueId == issueId;
            if (linkDirection == LinkDirectionEnum.All) return _issueLink => _issueLink.InwardIssueId == issueId
                                                                          || _issueLink.OutwardIssueId == issueId;

            return _issueLink => false;
        }
    }
}
