using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Services
{
    public interface IIssueTypeService
    {
        /// <summary>
        /// Returns all the issue types within lazyzu.Jira.
        /// </summary>
        Task<IEnumerable<IIssueType>> GetIssueTypesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the issue types within JIRA for the project specified.
        /// </summary>
        IAsyncEnumerable<IIssueType> GetIssueTypesForProjectAsync(string projectKey, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the issue types within JIRA for the project specified.
        /// </summary>
        IAsyncEnumerable<IIssueType> GetIssueTypesForProjectAsync(decimal projectId, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class IssueTypeService : IIssueTypeService
    {
        protected readonly JiraContext jiraContext;
        protected readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public IssueTypeService(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<IEnumerable<IIssueType>> GetIssueTypesAsync(CancellationToken cancellationToken = default)
        {
            if (cache.IssueTypes.Any() == false)
            {
                var query = jiraContext.issuetype.AsNoTracking()
                    .OrderBy(dbModel => dbModel.SEQUENCE)
                    .Select(dbModel => new
                    {
                        dbModel.ID,
                        dbModel.pname,
                        dbModel.DESCRIPTION,
                        dbModel.pstyle
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                foreach (var typeTempModel in queryResult)
                {
                    var isSubTask = "jira_subtask".Equals(typeTempModel.pstyle);
                    cache.IssueTypes.TryAdd(typeTempModel.ID, new IssueType
                    {
                        Id = typeTempModel.ID,
                        Name = typeTempModel.pname,
                        Description = typeTempModel.DESCRIPTION,
                        IsSubTask = isSubTask,
                        pstyle = typeTempModel.pstyle
                    });
                }
            }

            return cache.IssueTypes.Values;
        }

        public async IAsyncEnumerable<IIssueType> GetIssueTypesForProjectAsync(string projectKey, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var projectKeyService = jiraDatabaseQuerierGetter().Project.ProjectKey;
            var projectId = await projectKeyService.GetProjectIdAsync(projectKey, cancellationToken).ConfigureAwait(false);

            if (projectId.HasValue)
            {
                await foreach (var issueType in GetIssueTypesForProjectAsync(projectId.Value, cancellationToken)) yield return issueType;
            }
        }

        public async IAsyncEnumerable<IIssueType> GetIssueTypesForProjectAsync(decimal projectId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var query = from nodeassociation in jiraContext.nodeassociation.AsNoTracking()
                        from issuetypescreenschemeentity in jiraContext.issuetypescreenschemeentity.AsNoTracking().Where(_issuetypescreenschemeentity => _issuetypescreenschemeentity.SCHEME == nodeassociation.SINK_NODE_ID)
                        where nodeassociation.SOURCE_NODE_ID == projectId
                        && nodeassociation.SINK_NODE_ENTITY == "IssueTypeScreenScheme"
                        select issuetypescreenschemeentity.ISSUETYPE;

            var queryResult = await query.ToArrayAsync(cancellationToken);

            if (queryResult.Length != 0)
            {
                if (cache.IssueTypes.Any() == false) await this.GetIssueTypesAsync(cancellationToken).ConfigureAwait(false);   // include cache init
                foreach (var issueTypeId in queryResult)
                {
                    if (cache.IssueTypes.TryGetValue(issueTypeId, out var issueType)) yield return issueType;
                }
            }
        }
    }
}
