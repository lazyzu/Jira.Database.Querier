using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.User;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace lazyzu.Jira.Database.Querier
{
    public interface IJiraDatabaseQuerier : IDisposable
    {
        IIssueService Issue { get; }
        IProjectService Project { get; }
        IUserService User { get; }
    }

    public class JiraDatabaseQuerier : IJiraDatabaseQuerier
    {
        internal protected readonly JiraContext JiraContext;
        internal protected readonly ILogger Logger;
        internal protected readonly SharedCache Cache;

        internal protected readonly bool  leaveContextOpen;
        internal protected bool disposedValue;

        public IIssueService Issue { get; private init; }
        public IProjectService Project { get; private init; }
        public IUserService User { get; private init; }

        public JiraDatabaseQuerier(JiraContext jiraContext, SharedCache cache, ILogger logger, Func<IJiraDatabaseQuerier, ServicesBuildResponse> servicesBuilder, bool leaveContextOpen)
        {
            if (jiraContext == null) throw new ArgumentNullException(nameof(jiraContext));

            this.JiraContext = jiraContext;
            this.Cache = cache ?? new SharedCache();
            this.Logger = logger;
            this.leaveContextOpen = leaveContextOpen;

            var servicesBuildResponse = servicesBuilder?.Invoke(this);
            this.Issue = servicesBuildResponse?.Issue;
            this.Project = servicesBuildResponse?.Project;
            this.User = servicesBuildResponse?.User;
        }

        public class ServicesBuildResponse
        {
            public IIssueService Issue { get; set; }

            public IProjectService Project { get; set; }

            public IUserService User { get; set; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    if(leaveContextOpen == false) JiraContext.Dispose();
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~JiraDatabaseQuerier()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class SharedCache
    {
        /// <summary>
        /// IssueKey-IssueId Map
        /// </summary>
        public ConcurrentDictionary<string, IssueIdMapRecord> IssueKeyRecords = new ConcurrentDictionary<string, IssueIdMapRecord>();

        public class IssueIdMapRecord
        {
            public decimal IssueId { get; init; }
            public bool Moved { get; set; }
        }

        public ConcurrentDictionary<string, IIssueType> IssueTypes { get; } = new ConcurrentDictionary<string, IIssueType>();
        public ConcurrentDictionary<string, IIssuePriority> Priorities { get; } = new ConcurrentDictionary<string, IIssuePriority>();
        public ConcurrentDictionary<string, IIssueStatus> Statuses { get; } = new ConcurrentDictionary<string, IIssueStatus>();
        public ConcurrentDictionary<decimal, IIssueStatusCategory> StatusCategories { get; } = new ConcurrentDictionary<decimal, IIssueStatusCategory>();
        public ConcurrentDictionary<string, IIssueResolution> Resolutions { get; } = new ConcurrentDictionary<string, IIssueResolution>();
        public ConcurrentDictionary<decimal, IIssueSecurityLevelScheme> SecurityLevelSchemes { get; } = new ConcurrentDictionary<decimal, IIssueSecurityLevelScheme>();
        public ConcurrentDictionary<decimal, IIssueSecurityLevel> SecurityLevels { get; } = new ConcurrentDictionary<decimal, IIssueSecurityLevel>();

        public ConcurrentDictionary<decimal, IIssueLinkType> LinkTypes = new ConcurrentDictionary<decimal, IIssueLinkType>();

        public ConcurrentDictionary<string, decimal> ProjectIds = new ConcurrentDictionary<string, decimal>();
        public ConcurrentDictionary<decimal, IProjectCategory> ProjectCategories = new ConcurrentDictionary<decimal, IProjectCategory>();
        public ConcurrentDictionary<decimal, IProjectRole> ProjectRoles = new ConcurrentDictionary<decimal, IProjectRole>();
    }
}
