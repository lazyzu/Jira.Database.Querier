using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Issue.Services;
using lazyzu.Jira.Database.Querier.Project.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Fields
{
    public class ProjectIssueSecurityLevelProjection : IProjectExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly Func<IIssueSecurityLevelService> securityLevelServiceGetter;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectIssueSecurityLevelProjection(JiraContext jiraContext, Func<IIssueSecurityLevelService> securityLevelServiceGetter, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.securityLevelServiceGetter = securityLevelServiceGetter;
            this.cache = cache;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectIssueSecurityLevel
            };
        }

        public ProjectIssueSecurityLevelProjection(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache, ILogger logger)
            : this(jiraContext, () => jiraDatabaseQuerierGetter().Issue.IssueSecurityLevel, cache, logger) 
        { }

        public virtual async Task Projection(IEnumerable<JiraProject> projects, CancellationToken cancellationToken = default)
        {
            var _projects = projects?.ToArray() ?? new JiraProject[0];

            if (_projects.Any())
            {
                var projectIds = _projects.Select<JiraProject, decimal?>(project => project.Id).ToArray();

                var projectIssueSecurityLevels = await LoadProjectIssueSecurityLevels(projectIds, cancellationToken).ConfigureAwait(false);

                foreach (var project in _projects)
                {
                    if (projectIssueSecurityLevels.TryGetValue(project.Id, out var securityLevels)) project.SecurityLevels = securityLevels;
                    else project.SecurityLevels = new IIssueSecurityLevel[0];
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, IIssueSecurityLevel[]>> LoadProjectIssueSecurityLevels(decimal?[] projectIds, CancellationToken cancellationToken)
        {
            var projectIssueSecurityLevelSchemaQuery = from nodeassociation in jiraContext.nodeassociation.AsNoTracking()
                    where projectIds.Contains(nodeassociation.SOURCE_NODE_ID)
                     && nodeassociation.SINK_NODE_ENTITY == "IssueSecurityScheme"
                    select new
                    {
                        nodeassociation.SOURCE_NODE_ID,
                        nodeassociation.SINK_NODE_ID
                    };

            var projectIssueSecurityLevelSchemaQueryResult = await projectIssueSecurityLevelSchemaQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var securityLevelSchemaIds = projectIssueSecurityLevelSchemaQueryResult.Select(dbModel => dbModel.SINK_NODE_ID).ToArray();

            var securityLevelsIfScheme = await securityLevelServiceGetter().GetSecurityLevelsAsync(securityLevelSchemaIds, cancellationToken: cancellationToken);

            return projectIssueSecurityLevelSchemaQueryResult.ToDictionary(tempModel => tempModel.SOURCE_NODE_ID
            , tempModel =>
            {
                if (securityLevelsIfScheme.TryGetValue(tempModel.SINK_NODE_ID, out var securityLevels)) return securityLevels;
                else return new IIssueSecurityLevel[0];
            });
        }
    }
}
