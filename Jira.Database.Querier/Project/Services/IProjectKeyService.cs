using lazyzu.Jira.Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Services
{
    public interface IProjectKeyService
    {
        Task<decimal?> GetProjectIdAsync(string projectKey, CancellationToken cancellationToken = default);
        Task<Dictionary<string, decimal>> GetProjectIdsAsync(IEnumerable<string> projectKeys, CancellationToken cancellationToken = default);
    }

    public class ProjectKeyService : IProjectKeyService
    {
        protected readonly JiraContext jiraContext;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectKeyService(JiraContext jiraContext, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<decimal?> GetProjectIdAsync(string projectKey, CancellationToken cancellationToken = default)
        {
            if (cache.ProjectIds.TryGetValue(projectKey, out var projectId)) return projectId;
            else
            {
                var query = jiraContext.project_key.AsNoTracking()
                    .Where(project_key => project_key.PROJECT_KEY1 == projectKey)
                    .Select(project_key => project_key.PROJECT_ID);

                var queryResult = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

                if (queryResult.HasValue) cache.ProjectIds.TryAdd(projectKey, queryResult.Value);
                return queryResult;
            }
        }

        public async Task<Dictionary<string, decimal>> GetProjectIdsAsync(IEnumerable<string> projectKeys, CancellationToken cancellationToken = default)
        {
            var _projectKeys = projectKeys?.Distinct()?.ToArray() ?? new string[0];

            if (_projectKeys.Any())
            {
                var missingKeys = _projectKeys.Except(cache.ProjectIds.Keys).ToArray();

                var query = jiraContext.project_key.AsNoTracking()
                    .Where(project_key => missingKeys.Contains(project_key.PROJECT_KEY1))
                    .Select(project_key => new
                    {
                        project_key.PROJECT_ID,
                        project_key.PROJECT_KEY1
                    });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                foreach (var dbModel in queryResult)
                {
                    cache.ProjectIds.TryAdd(dbModel.PROJECT_KEY1, dbModel.PROJECT_ID.Value);
                }

                return cache.ProjectIds.Where(map => _projectKeys.Contains(map.Key))
                    .ToDictionary(map => map.Key, map => map.Value);
            }
            else return new Dictionary<string, decimal>();
        }
    }
}
