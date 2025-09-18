using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public interface IIssueRemoteLink : IEquatable<IIssueRemoteLink>
    {
        decimal Id { get; }
        string RemoteUrl { get; }
        string Title { get; }
        string Summary { get; }
        string Relationship { get; }
    }

    public class IssueRemoteLink : IIssueRemoteLink
    {
        public decimal Id { get; init; }

        public string RemoteUrl { get; init; }

        public string Title { get; init; }

        public string Summary { get; init; }

        public string Relationship { get; init; }

        public static bool operator ==(IssueRemoteLink left, IIssueConfluenceLink right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(IssueRemoteLink left, IIssueConfluenceLink right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IIssueRemoteLink);
        }

        public bool Equals(IIssueRemoteLink other)
        {
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return RemoteUrl;
        }
    }

    public record ConfluenceInstance(string Name, string AppId);

    public interface IIssueConfluenceLink : IIssueRemoteLink
    {
        ConfluenceInstance ConfluenceInstance { get; }
        string PageId { get; }
    }

    public class IssueConfluenceLink : IIssueConfluenceLink
    {
        public ConfluenceInstance ConfluenceInstance { get; init; }

        public string PageId { get; init; }

        public decimal Id { get; init; }

        public string RemoteUrl { get; init; }

        public string Title { get; init; }

        public string Summary { get; init; }

        public string Relationship { get; init; }

        public static bool operator ==(IssueConfluenceLink left, IIssueConfluenceLink right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(IssueConfluenceLink left, IIssueConfluenceLink right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IIssueRemoteLink);
        }

        public bool Equals(IIssueRemoteLink other)
        {
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return RemoteUrl;
        }
    }

    public class IssueRemoteLinkProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly IRemoteLinkResolver[] remoteLinkResolvers;
        protected readonly IRemoteLinkResolver defaultRemoteLinkResolver;
        protected readonly ILogger logger;

        public IssueRemoteLinkProjection(JiraContext jiraContext, IRemoteLinkResolver[] remoteLinkResolvers, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.remoteLinkResolvers = remoteLinkResolvers;
            this.defaultRemoteLinkResolver = new DefaultRemoteLinkResolver();
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.RemoteLinks
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueRemoteLinkMap = await LoadIssueVersionMap(issueIds, cancellationToken).ConfigureAwait(false);

                if (issueRemoteLinkMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueRemoteLinkMap.TryGetValue(issue.Id, out var remoteLinks)) issue.RemoteLinks = remoteLinks;
                        else issue.RemoteLinks = new IIssueRemoteLink[0];
                    }
                }
                else foreach (var issue in _issues)
                {
                    issue.RemoteLinks = new IIssueRemoteLink[0];
                }
            }
        }

        protected virtual async Task<Dictionary<decimal?, IIssueRemoteLink[]>> LoadIssueVersionMap(decimal?[] issueIds, CancellationToken cancellationToken)
        {
            var query = jiraContext.remotelink.AsNoTracking()
                .Where(remotelink => issueIds.Contains(remotelink.ISSUEID))
                .Select(remotelink => new RemoteLinkQueryModel(
                    remotelink.ID,
                    remotelink.ISSUEID,
                    remotelink.GLOBALID,
                    remotelink.TITLE,
                    remotelink.SUMMARY,
                    remotelink.URL,
                    remotelink.RELATIONSHIP,
                    remotelink.APPLICATIONTYPE,
                    remotelink.APPLICATIONNAME
                ));

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.GroupBy(dbModel => dbModel.ISSUEID)
                .ToDictionary(issueIdGroup => issueIdGroup.Key
                            , issueIdGroup => issueIdGroup.Select(dbModel => ResolveWithResolvers(dbModel)).ToArray());
        }

        protected virtual IIssueRemoteLink ResolveWithResolvers(RemoteLinkQueryModel queryModel)
        {
            foreach (var remoteLinkResolver in remoteLinkResolvers)
            {
                if (remoteLinkResolver.TryToResolve(queryModel, out var result)) return result;
            }

            defaultRemoteLinkResolver.TryToResolve(queryModel, out var defaultResult);
            return defaultResult;
        }
    }

    public interface IRemoteLinkResolver
    {
        public bool TryToResolve(RemoteLinkQueryModel queryModel, out IIssueRemoteLink issueRemoteLink);
    }

    public record RemoteLinkQueryModel(decimal ID
        , decimal? ISSUEID
        , string GLOBALID
        , string TITLE
        , string SUMMARY
        , string URL
        , string RELATIONSHIP
        , string APPLICATIONTYPE
        , string APPLICATIONNAME);

    public class DefaultRemoteLinkResolver : IRemoteLinkResolver
    {
        public bool TryToResolve(RemoteLinkQueryModel queryModel, out IIssueRemoteLink issueRemoteLink)
        {
            issueRemoteLink = new IssueRemoteLink
            {
                Id = queryModel.ID,
                RemoteUrl = queryModel.URL,
                Title = queryModel.TITLE,
                Summary = queryModel.SUMMARY,
                Relationship = queryModel.RELATIONSHIP,
            };

            return true;
        }
    }

    public class ConfluenceLinkResolver : IRemoteLinkResolver
    {
        public virtual bool TryToResolve(RemoteLinkQueryModel queryModel, out IIssueRemoteLink issueRemoteLink)
        {
            issueRemoteLink = null;

            var isFromConfluence = "com.atlassian.confluence".Equals(queryModel?.APPLICATIONTYPE);
            if (isFromConfluence == false) return false;

            var isGlobalIdResolved = TryToResolveGlobalId(queryModel.GLOBALID, out var appId, out var pageId);
            if(isGlobalIdResolved == false) return false;

            issueRemoteLink = new IssueConfluenceLink
            {
                Id = queryModel.ID,
                RemoteUrl = queryModel.URL,
                Title = queryModel.TITLE,
                Summary = queryModel.SUMMARY,
                Relationship = queryModel.RELATIONSHIP,
                ConfluenceInstance = new ConfluenceInstance(queryModel.APPLICATIONNAME, appId),
                PageId = pageId,
            };
            return true;
        }

        protected virtual bool TryToResolveGlobalId(string globalId, out string appId, out string pageId)
        {
            appId = null;
            pageId = null;

            if (string.IsNullOrEmpty(globalId)) return false;

            var globalIdValues = HttpUtility.ParseQueryString(globalId);
            appId = globalIdValues["appId"];
            pageId = globalIdValues["pageId"];

            return (string.IsNullOrEmpty(appId) == false) && (string.IsNullOrEmpty(pageId) == false);
        }
    }
}
