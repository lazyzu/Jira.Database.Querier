using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.User.Fields;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace lazyzu.Jira.Database.Querier
{
    public partial class JiraDatabaseQuerierBuilder
    {
        protected readonly Uri BaseUri;
        protected SharedCache Cache;

        public JiraDatabaseQuerierBuilder(Uri baseUri)
        {
            this.BaseUri = baseUri;
            this.Cache = new SharedCache();
        }

        public IJiraDatabaseQuerier Build(JiraContext jiraContext, ILogger logger, bool resetCache = false, bool leaveContextOpen = true)
        {
            if (resetCache) Cache = new SharedCache();

            return new JiraDatabaseQuerier(jiraContext, Cache, logger, servicesBuilder: jiraDatabaseQuerier =>
            {
                var context = new ServiceBuildContext()
                {
                    JiraContext = jiraContext,
                    JiraDatabaseQuerierGetter = () => jiraDatabaseQuerier,
                    IssueAttachmentUrlBuilder = IssueAttachmentUrlBuilder?.Invoke(BaseUri, logger) ?? new IssueAttachmentUrlBuilder(BaseUri),
                    ProjectAvatarUrlBuilder = ProjectAvatarUrlBuilderConstructor?.Invoke(BaseUri, logger) ?? new ProjectAvatarUrlBuilder(BaseUri),
                    UserAvatarUrlBuilder = UserAvatarUrlBuilderConstructor?.Invoke(BaseUri, logger) ?? new UserAvatarUrlBuilder(BaseUri),
                    Cache = Cache,
                    Logger = logger,
                };

                var issueFieldServiceCollection = BuildIssueFieldServiceCollection(context);
                var projectFieldServiceCollection = BuildProjectFieldServiceCollection(context);

                return new JiraDatabaseQuerier.ServicesBuildResponse
                {
                    Issue = BuildIssueService(context, issueFieldServiceCollection),
                    Project = BuildProjectService(context, projectFieldServiceCollection),
                    User = BuildUserService(context)
                };
            }, leaveContextOpen);
        }

        public IJiraDatabaseQuerier Build(JiraContextGetterDelegate jiraContextGetter, ILogger logger, bool resetCache = false, bool leaveContextOpen = false)
            => Build(jiraContextGetter(), logger, resetCache, leaveContextOpen);

        public delegate JiraContext JiraContextGetterDelegate();
        public delegate T BuildServiceDelegate<T>(ServiceBuildContext context);
        public delegate IEnumerable<T> BuildSpecificationDelegate<T>(ServiceBuildContext context);
        public class ServiceBuildContext()
        {
            public JiraContext JiraContext { get; init; }
            public Func<IJiraDatabaseQuerier> JiraDatabaseQuerierGetter { get; init; }
            public IIssueAttachmentUrlBuilder IssueAttachmentUrlBuilder { get; init; }
            public IProjectAvatarUrlBuilder ProjectAvatarUrlBuilder { get; init; }
            public IUserAvatarUrlBuilder UserAvatarUrlBuilder { get; init; }
            public SharedCache Cache { get; init; }
            public ILogger Logger { get; init; }
        }
    }
}
