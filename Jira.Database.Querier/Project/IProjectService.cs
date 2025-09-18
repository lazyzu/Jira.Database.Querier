using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Project.Contract;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.Project.Services;
using lazyzu.Jira.Database.Querier.ProjectionSpecification;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project
{
    public interface IProjectFieldServiceCollection
    {
        IProjectKeyService ProjectKey { get; }
        IProjectCategoryService ProjectCategory { get; }
        IProjectRoleService ProjectRole { get; }
    }

    public interface IProjectService : IProjectFieldServiceCollection
    {
        ImmutableArray<FieldKey> DefaultQueryFields { get; }

        Task<IJiraProject[]> SearchProjectAsync(Func<IProjectSpecs, IQuerySpecification[]> projectSearchSpecificationBuilder, FieldKey[] fields = null, CancellationToken cancellationToken = default);

        Task<IJiraProject[]> GetProjectsAsync(IEnumerable<string> projectKeys, FieldKey[] fields = null, CancellationToken cancellationToken = default);

        Task<IJiraProject> GetProjectAsync(string projectKey, FieldKey[] fields = null, CancellationToken cancellationToken = default);

        Task<IJiraProject[]> GetProjectsAsync(IEnumerable<decimal> projectIds, FieldKey[] fields = null, CancellationToken cancellationToken = default);

        Task<IJiraProject> GetProjectAsync(decimal projectId, FieldKey[] fields = null, CancellationToken cancellationToken = default);
    }

    public class ProjectService : IProjectService
    {
        public IProjectKeyService ProjectKey { get; init; }
        public IProjectCategoryService ProjectCategory { get; init; }
        public IProjectRoleService ProjectRole { get; init; }

        protected readonly JiraContext jiraContext;
        public ImmutableArray<FieldKey> DefaultQueryFields { get; protected init; }
        protected readonly IProjectProjectionSpecification[] projectProjectionSpecifications;
        protected readonly Dictionary<FieldKey, IProjectProjectionSpecification> projectProjectionMap;
        protected readonly IProjectQuerySpecificationHandler[] projectQuerySpecificationHandlers;
        protected readonly Dictionary<Type, IProjectQuerySpecificationHandler> querySpecificationHandlerMap;
        protected readonly IProjectSpecs projectSpecs;
        protected readonly ILogger logger;

        public ProjectService(JiraContext jiraContext
            , IProjectProjectionSpecification[] projectProjectionSpecifications
            , IProjectQuerySpecificationHandler[] projectQuerySpecificationHandlers
            , FieldKey[] defaultQueryFields
            , IProjectSpecs projectSpecs
            , ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.projectProjectionSpecifications = projectProjectionSpecifications ?? new IProjectProjectionSpecification[0];
            this.projectProjectionMap = ToProjectProjectionMap(this.projectProjectionSpecifications);
            this.projectQuerySpecificationHandlers = projectQuerySpecificationHandlers ?? new IProjectQuerySpecificationHandler[0];
            this.querySpecificationHandlerMap = ToQuerySpecificationHandlerMap(this.projectQuerySpecificationHandlers);
            this.DefaultQueryFields = defaultQueryFields?.ToImmutableArray() ?? ImmutableArray<FieldKey>.Empty;
            this.projectSpecs = projectSpecs;
            this.logger = logger;
        }

        protected virtual Dictionary<FieldKey, IProjectProjectionSpecification> ToProjectProjectionMap(IProjectProjectionSpecification[] projectProjectionSpecifications)
        {
            var result = new Dictionary<FieldKey, IProjectProjectionSpecification>();
            if (projectProjectionSpecifications.Any())
            {
                foreach (var projectProjectionSpec in projectProjectionSpecifications)
                {
                    var handleTargets = projectProjectionSpec.HandleTarget.ToArray();
                    foreach (var handleTarget in handleTargets)
                    {
                        result.TryAdd(handleTarget, projectProjectionSpec);
                    }
                }
            }
            result.TryAdd(ProjectFieldSelection.ProjectId, new ProjectIdProjection());
            return result;
        }

        protected virtual Dictionary<Type, IProjectQuerySpecificationHandler> ToQuerySpecificationHandlerMap(IProjectQuerySpecificationHandler[] querySpecificationHandlers)
        {
            var result = new Dictionary<Type, IProjectQuerySpecificationHandler>();
            if (querySpecificationHandlers != null)
            {
                foreach (var querySpecificationHandler in querySpecificationHandlers)
                {
                    var handleSchemaTargets = querySpecificationHandler.HandleSchemaTarget.ToArray();
                    foreach (var handleSchemaTarget in handleSchemaTargets)
                    {
                        result.TryAdd(handleSchemaTarget, querySpecificationHandler);
                    }
                }
            }
            return result;
        }

        public virtual async Task<IJiraProject[]> SearchProjectAsync(Func<IProjectSpecs, IQuerySpecification[]> projectSearchSpecificationBuilder, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var projectSearchSpecifications = projectSearchSpecificationBuilder?.Invoke(projectSpecs) ?? new IQuerySpecification[0];

            var optimizedProjectSearchSpecifications = ProjectSearchSpecificationExtension.BuildOptimize(projectSearchSpecifications, querySpecificationHandlerMap);
            if (optimizedProjectSearchSpecifications.Any())
            {
                decimal[] projectIds = null;
                foreach (var querySpecification in optimizedProjectSearchSpecifications)
                {
                    if (querySpecificationHandlerMap.TryGetValue(querySpecification.SchemaType, out var querySpecificationHandler))
                    {
                        projectIds = await querySpecificationHandler.SearchIssueAsync(jiraContext, querySpecification, projectIds, cancellationToken).ConfigureAwait(false);
                    }
                    else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported issue spec");

                    if (projectIds != null && projectIds.Length == 0) break;
                }

                return await GetProjectsAsync(projectIds, fields, cancellationToken).ConfigureAwait(false);
            }
            else return new IJiraProject[0];
        }

        public virtual async Task<IJiraProject[]> GetProjectsAsync(IEnumerable<string> projectKeys, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var projectIds = (await ProjectKey.GetProjectIdsAsync(projectKeys, cancellationToken).ConfigureAwait(false)).Values.ToArray();
            return await GetProjectsAsync(projectIds, fields, cancellationToken).ConfigureAwait(false); 
        }

        public virtual async Task<IJiraProject[]> GetProjectsAsync(IEnumerable<decimal> projectIds, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var _projectIds = projectIds?.ToArray() ?? new decimal[0];
            if (_projectIds.Any())
            {
                var _fields = InitFieldSelection(fields);
                var projectProjections = LoadProjectProjection(_fields).ToArray();
                var projectProjectionSelection = ProjectProjectionSelection.InitialFrom(projectProjections);

                var query = jiraContext.project.AsNoTracking()
                    .Where(project => _projectIds.Contains(project.ID))
                    .ProjectionInclude(projectProjectionSelection);

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                var dbModelProjectionMaps = queryResult.ToDictionary(dbModel => dbModel, dbModel => new JiraProject());

                foreach (var field in projectProjectionSelection.SimpleProjections)
                {
                    foreach (var dbModelProjectionMap in dbModelProjectionMaps)
                    {
                        var dbModel = dbModelProjectionMap.Key;
                        var projectionProjection = dbModelProjectionMap.Value;
                        await field.Projection(dbModel, projectionProjection, cancellationToken).ConfigureAwait(false);
                    }
                }

                foreach (var field in projectProjectionSelection.ContextProjections)
                {
                    var fieldRenderContext = await field.PrepareContext(queryResult, cancellationToken);

                    foreach (var dbModelProjectionMap in dbModelProjectionMaps)
                    {
                        var dbModel = dbModelProjectionMap.Key;
                        var projectionProjection = dbModelProjectionMap.Value;
                        await field.Projection(dbModel, projectionProjection, fieldRenderContext, cancellationToken).ConfigureAwait(false);
                    }
                }

                var resultProjections = dbModelProjectionMaps.Values.ToArray();

                foreach (var field in projectProjectionSelection.ExternalProjections)
                {
                    await field.Projection(resultProjections, cancellationToken).ConfigureAwait(false);
                }

                return resultProjections;
            }
            else return new IJiraProject[0];
        }

        public virtual async Task<IJiraProject> GetProjectAsync(string projectKey, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var projectId = await ProjectKey.GetProjectIdAsync(projectKey, cancellationToken).ConfigureAwait(false);
            if (projectId.HasValue) return await GetProjectAsync(projectId.Value, fields, cancellationToken).ConfigureAwait(false);
            else return default;
        }

        public async Task<IJiraProject> GetProjectAsync(decimal projectId, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var _fields = InitFieldSelection(fields);
            var projectProjections = LoadProjectProjection(_fields).ToArray();
            var projectProjectionSelection = ProjectProjectionSelection.InitialFrom(projectProjections);

            var query = jiraContext.project.AsNoTracking()
                .Where(project => project.ID == projectId)
                .ProjectionInclude(projectProjectionSelection);

            var queryResult = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (queryResult == default) return default;

            var resultProjectProjection = new JiraProject();

            foreach (var field in projectProjectionSelection.SimpleProjections)
            {
                await field.Projection(queryResult, resultProjectProjection, cancellationToken).ConfigureAwait(false);
            }

            foreach (var field in projectProjectionSelection.ContextProjections)
            {
                var fieldRenderContext = await field.PrepareContext(new project[]
                {
                    queryResult
                }, cancellationToken);

                await field.Projection(queryResult, resultProjectProjection, fieldRenderContext, cancellationToken).ConfigureAwait(false);
            }

            foreach (var field in projectProjectionSelection.ExternalProjections)
            {
                await field.Projection(new JiraProject[]
                {
                    resultProjectProjection
                }, cancellationToken).ConfigureAwait(false);
            }

            return resultProjectProjection;
        }

        protected virtual ImmutableArray<FieldKey> InitFieldSelection(IEnumerable<FieldKey> fields)
        {
            IEnumerable<FieldKey> _fields = fields;
            if (_fields == null) _fields = DefaultQueryFields;

            return _fields.Concat(new FieldKey[]
            {
                ProjectFieldSelection.ProjectId
            }).ToImmutableArray();
        }

        protected virtual IEnumerable<IProjectProjectionSpecification> LoadProjectProjection(ImmutableArray<FieldKey> fields)
        {
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    if (projectProjectionMap.TryGetValue(field, out var projectProjection))
                    {
                        if (projectProjection is IProjectProjectionSpecificationFieldKeyDependency fieldKeyDependencyProjection) yield return fieldKeyDependencyProjection.ConstructFrom(field, fields);
                        else yield return projectProjection;
                    }
                    else throw new NotSupportedException($"{field.Name} is not supported by injected projection specification");
                }
            }
        }
    }

    public static class ProjectSearchSpecificationExtension
    {
        public static IQuerySpecification[] BuildOptimize(IEnumerable<IQuerySpecification> specs, Dictionary<Type, IProjectQuerySpecificationHandler> querySpecificationHandlerMap)
        {
            if (specs != null && specs.Any())
            {
                var schemaTypeSpecMap = specs.GroupBy(spec => spec.SchemaType).ToDictionary(schemaTypeSpecGroup => schemaTypeSpecGroup.Key, schemaTypeSpecGroup => schemaTypeSpecGroup.ToArray());

                return OrderDistinctSchemaType(specs).Select(schemaType =>
                {
                    var querySpecificationHandler = querySpecificationHandlerMap[schemaType];
                    var querySpecifications = schemaTypeSpecMap[schemaType];
                    return querySpecificationHandler.Union(querySpecifications);

                }).ToArray();
            }
            else return new IQuerySpecification[0];
        }

        public static List<Type> OrderDistinctSchemaType(IEnumerable<IQuerySpecification> specs)
        {
            var result = new List<Type>();
            foreach (var spec in specs)
            {
                if (result.Contains(spec.SchemaType) == false) result.Add(spec.SchemaType);
            }
            return result;
        }
    }
    public class ProjectProjectionSelection
    {
        public IProjectProjectionSpecification<project>[] SimpleProjections { get; private init; }
        public IProjectProjectionWithContextSpecification<project>[] ContextProjections { get; private init; }
        public IProjectExternalProjectionSpecification[] ExternalProjections { get; private init; }

        public static ProjectProjectionSelection InitialFrom(IProjectProjectionSpecification[] projectProjections)
        {
            if (projectProjections == null || projectProjections.Length == 0) throw new ArgumentNullException(nameof(projectProjections));

            var simpleProjections = new List<IProjectProjectionSpecification<project>>();
            var contextProjections = new List<IProjectProjectionWithContextSpecification<project>>();
            var externalProjections = new List<IProjectExternalProjectionSpecification>();

            foreach (var projectProjection in projectProjections)
            {
                switch (projectProjection)
                {
                    case IProjectProjectionSpecification<project> simpleProjection:
                        simpleProjections.Add(simpleProjection);
                        break;
                    case IProjectProjectionWithContextSpecification<project> contextProjection:
                        contextProjections.Add(contextProjection);
                        break;
                    case IProjectExternalProjectionSpecification externalProjection:
                        externalProjections.Add(externalProjection);
                        break;
                    default:
                        throw new NotSupportedException($"{projectProjection.GetType().Name} is not supported yet");
                }
            }

            return new ProjectProjectionSelection
            {
                SimpleProjections = simpleProjections.ToArray(),
                ContextProjections = contextProjections.ToArray(),
                ExternalProjections = externalProjections.ToArray()
            };
        }
    }

    public static class ProjectProjectionExtension
    {
        public static IQueryable<project> ProjectionInclude(this IQueryable<project> inputQuery, ProjectProjectionSelection projectProjection)
        {
            var projectionIncludeSpecifications = projectProjection.SimpleProjections.Concat<IProjectionIncludeSpecification<project>>(projectProjection.ContextProjections);
            return inputQuery.ProjectionInclude(projectionIncludeSpecifications);
        }
    }
}
