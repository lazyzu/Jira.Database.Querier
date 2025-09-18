using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Issue.Services;
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

namespace lazyzu.Jira.Database.Querier.Issue
{
    public interface IIssueFieldServiceCollection
    {
        IIssueKeyService IssueKey { get; }
        IIssuePriorityService IssuePriority { get; }
        IIssueResolutionService IssueResolution { get; }
        IIssueSecurityLevelService IssueSecurityLevel { get; }
        IIssueStatusService IssueStatus { get; }
        IIssueStatusCategoryService IssueStatusCategory { get; }
        IIssueTypeService IssueType { get; }
        IIssueLinkService IssueLink { get; }
        IIssueCustomFieldService IssueCustomField { get; }
    }

    public interface IIssueService : IIssueFieldServiceCollection
    {
        ImmutableArray<FieldKey> DefaultQueryFields { get; }

        Task<IJiraIssue> GetIssueAsync(decimal issueId, FieldKey[] fields = null, CancellationToken cancellationToken = default);
        Task<IJiraIssue> GetIssueAsync(string issueKey, FieldKey[] fields = null, CancellationToken cancellationToken = default);

        Task<IJiraIssue[]> GetIssuesAsync(IEnumerable<decimal> issueIds, FieldKey[] fields = null, CancellationToken cancellationToken = default);
        Task<IJiraIssue[]> GetIssuesAsync(IEnumerable<string> issueKeys, FieldKey[] fields = null, CancellationToken cancellationToken = default);

        Task<decimal[]> SearchIssueIdAsync(Func<IIssueSpecs, IQuerySpecification[]> issueSearchSpecificationBuilder, CancellationToken cancellationToken = default);
        Task<IJiraIssue[]> SearchIssueAsync(Func<IIssueSpecs, IQuerySpecification[]> issueSearchSpecificationBuilder, FieldKey[] fields = null, CancellationToken cancellationToken = default);
    }

    public class IssueService : IIssueService
    {
        public IIssueKeyService IssueKey { get; init; }
        public IIssuePriorityService IssuePriority { get; init; }
        public IIssueResolutionService IssueResolution { get; init; }
        public IIssueSecurityLevelService IssueSecurityLevel { get; init; }
        public IIssueStatusService IssueStatus { get; init; }
        public IIssueStatusCategoryService IssueStatusCategory { get; init; }
        public IIssueTypeService IssueType { get; init; }
        public IIssueLinkService IssueLink { get; init; }
        public IIssueCustomFieldService IssueCustomField { get; init; }

        protected readonly JiraContext jiraContext;
        public ImmutableArray<FieldKey> DefaultQueryFields { get; protected init; }
        protected readonly IIssueProjectionSpecification[] fieldProjectionSpecifications;
        protected readonly Dictionary<FieldKey, IIssueProjectionSpecification> fieldProjectionMap;
        protected readonly IIssueCustomFieldProjectionSpecification[] customFieldProjectionSpecifications;
        protected readonly IIssueQuerySpecificationHandler[] querySpecificationHandlers;
        protected readonly Dictionary<Type, IIssueQuerySpecificationHandler> querySpecificationHandlerMap;
        protected readonly IIssueSpecs issueSpecs;
        protected readonly ILogger logger;

        public IssueService(JiraContext jiraContext
            , IIssueProjectionSpecification[] fieldProjectionSpecifications
            , IIssueCustomFieldProjectionSpecification[] customFieldProjectionSpecifications
            , IIssueQuerySpecificationHandler[] querySpecificationHandlers
            , FieldKey[] defaultQueryFields
            , IIssueSpecs issueSpecs
            , ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.fieldProjectionSpecifications = fieldProjectionSpecifications ?? new IIssueProjectionSpecification[0];
            this.fieldProjectionMap = ToIssueFieldProjectionMap(this.fieldProjectionSpecifications);
            this.customFieldProjectionSpecifications = customFieldProjectionSpecifications ?? new IIssueCustomFieldProjectionSpecification[0];
            this.querySpecificationHandlers = querySpecificationHandlers ?? new IIssueQuerySpecificationHandler[0];
            this.querySpecificationHandlerMap = ToQuerySpecificationHandlerMap(this.querySpecificationHandlers);
            this.DefaultQueryFields = defaultQueryFields?.ToImmutableArray() ?? ImmutableArray<FieldKey>.Empty;
            this.issueSpecs = issueSpecs;
            this.logger = logger;
        }

        protected virtual Dictionary<FieldKey, IIssueProjectionSpecification> ToIssueFieldProjectionMap(IIssueProjectionSpecification[] issueProjectionSpecifications)
        {
            var result = new Dictionary<FieldKey, IIssueProjectionSpecification>();
            if (issueProjectionSpecifications != null)
            {
                foreach (var issueProjectionSpec in issueProjectionSpecifications)
                {
                    var handleTargets = issueProjectionSpec.HandleTarget.ToArray();
                    foreach (var handleTarget in handleTargets)
                    {
                        result.TryAdd(handleTarget, issueProjectionSpec);
                    }
                }
            }

            result.TryAdd(IssueFieldSelection.IssueId, new IssueIdProjection());
            return result;
        }

        protected virtual Dictionary<Type, IIssueQuerySpecificationHandler> ToQuerySpecificationHandlerMap(IIssueQuerySpecificationHandler[] querySpecificationHandlers)
        {
            var result = new Dictionary<Type, IIssueQuerySpecificationHandler>();
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

        public async Task<decimal[]> SearchIssueIdAsync(Func<IIssueSpecs, IQuerySpecification[]> issueSearchSpecificationBuilder, CancellationToken cancellationToken = default)
        {
            var issueSearchSpecifications = issueSearchSpecificationBuilder?.Invoke(issueSpecs) ?? new IQuerySpecification[0];

            var optimizedIssueSearchSpecifications = IssueSearchSpecificationExtension.BuildOptimize(issueSearchSpecifications, querySpecificationHandlerMap);
            if (optimizedIssueSearchSpecifications.Any())
            {
                decimal[] issueIds = null;
                foreach (var querySpecification in optimizedIssueSearchSpecifications)
                {
                    if (querySpecificationHandlerMap.TryGetValue(querySpecification.SchemaType, out var querySpecificationHandler))
                    {
                        issueIds = await querySpecificationHandler.SearchIssueAsync(jiraContext, querySpecification, issueIds, cancellationToken).ConfigureAwait(false);
                    }
                    else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported issue spec");

                    if (issueIds != null && issueIds.Length == 0) break;
                }
                return issueIds;
            }
            else return new decimal[0];
        }

        public virtual async Task<IJiraIssue[]> SearchIssueAsync(Func<IIssueSpecs, IQuerySpecification[]> issueSearchSpecificationBuilder, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var issueIds = await SearchIssueIdAsync(issueSearchSpecificationBuilder, cancellationToken).ConfigureAwait(false);

            if (issueIds.Any())
            {
                return await GetIssuesAsync(issueIds, fields, cancellationToken).ConfigureAwait(false);
            }
            else return new IJiraIssue[0];
        }

        public virtual async Task<IJiraIssue[]> GetIssuesAsync(IEnumerable<string> issueKeys, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var issueIds = (await IssueKey.GetIssueIdAsync(issueKeys, cancellationToken).ConfigureAwait(false)).Values.ToArray();
            return await GetIssuesAsync(issueIds, fields, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IJiraIssue[]> GetIssuesAsync(IEnumerable<decimal> issueIds, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var _issueIds = issueIds?.ToArray() ?? new decimal[0];

            if (_issueIds.Any())
            {
                var _fields = InitFieldSelection(fields);
                SeparateCustomField(_fields, out var simpleFields, out var customFields);
                var simpleFieldProjections = LoadSimpleFieldProjection(simpleFields).ToArray();
                var simpleFieldProjectionSelection = IssueProjection.InitialFrom(simpleFieldProjections);

                var query = jiraContext.jiraissue.AsNoTracking()
                    .Where(issue => _issueIds.Contains(issue.ID))
                    .ProjectionInclude(simpleFieldProjectionSelection);

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                var dbModelProjectionMaps = queryResult.ToDictionary(dbModel => dbModel, dbModel => new JiraIssue());

                foreach (var field in simpleFieldProjectionSelection.IssueSimpleProjections)
                {
                    foreach (var dbModelProjectionMap in dbModelProjectionMaps)
                    {
                        var dbModel = dbModelProjectionMap.Key;
                        var projectionProjection = dbModelProjectionMap.Value;
                        await field.Projection(dbModel, projectionProjection, cancellationToken).ConfigureAwait(false);
                    }
                }

                foreach (var field in simpleFieldProjectionSelection.IssueContextProjections)
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

                foreach (var field in simpleFieldProjectionSelection.IssueExternalProjections)
                {
                    await field.Projection(resultProjections, cancellationToken).ConfigureAwait(false);
                }

                var customFieldProjectionMap = LoadCustomFieldProjection(customFields);

                foreach (var field in customFields)
                {
                    if (customFieldProjectionMap.TryGetValue(field, out var projectionHandler))
                    {
                        await projectionHandler.Projection(field, resultProjections, cancellationToken).ConfigureAwait(false);
                    }
                    else throw new NotSupportedException($"{field.Name} is not supported by injected projection specification");
                }

                return resultProjections;
            }
            else return new IJiraIssue[0];
        }

        public virtual async Task<IJiraIssue> GetIssueAsync(string issueKey, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var issueId = await IssueKey.GetIssueIdAsync(issueKey, cancellationToken).ConfigureAwait(false);
            if (issueId.HasValue) return await GetIssueAsync(issueId.Value, fields, cancellationToken).ConfigureAwait(false);
            else return default;
        }

        public virtual async Task<IJiraIssue> GetIssueAsync(decimal issueId, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var _fields = InitFieldSelection(fields);
            SeparateCustomField(_fields, out var simpleFields, out var customFields);
            var simpleFieldProjections = LoadSimpleFieldProjection(simpleFields).ToArray();
            var simpleFieldProjectionSelection = IssueProjection.InitialFrom(simpleFieldProjections);

            var query = jiraContext.jiraissue.AsNoTracking()
                .Where(issue => issue.ID == issueId)
                .ProjectionInclude(simpleFieldProjectionSelection);

            var queryResult = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (queryResult == default) return default;

            var resultIssueProjection = new JiraIssue();

            foreach (var field in simpleFieldProjectionSelection.IssueSimpleProjections)
            {
                await field.Projection(queryResult, resultIssueProjection, cancellationToken).ConfigureAwait(false);
            }

            foreach (var field in simpleFieldProjectionSelection.IssueContextProjections)
            {
                var fieldRenderContext = await field.PrepareContext(new jiraissue[]
                {
                    queryResult
                }, cancellationToken);

                await field.Projection(queryResult, resultIssueProjection, fieldRenderContext, cancellationToken).ConfigureAwait(false);
            }

            foreach (var field in simpleFieldProjectionSelection.IssueExternalProjections)
            {
                await field.Projection(new JiraIssue[]
                {
                    resultIssueProjection
                }, cancellationToken).ConfigureAwait(false);
            }

            var customFieldProjectionMap = LoadCustomFieldProjection(customFields);

            foreach (var field in customFields)
            {
                if (customFieldProjectionMap.TryGetValue(field, out var projectionHandler))
                {
                    await projectionHandler.Projection(field, new JiraIssue[] 
                    { 
                        resultIssueProjection 
                    }, cancellationToken).ConfigureAwait(false);
                }
                else throw new NotSupportedException($"{field.Name} is not supported by injected projection specification");
            }

            return resultIssueProjection;
        }

        protected virtual ImmutableArray<FieldKey> InitFieldSelection(IEnumerable<FieldKey> fields)
        {
            IEnumerable<FieldKey> _fields = fields;
            if (_fields == null) _fields = DefaultQueryFields;

            return _fields.Concat(new FieldKey[]
            {
                IssueFieldSelection.IssueId
            }).ToImmutableArray();
        }

        protected virtual void SeparateCustomField(ImmutableArray<FieldKey> fields, out List<FieldKey> simpleFields, out List<ICustomFieldKey> customFields)
        {
            simpleFields = new List<FieldKey>();
            customFields = new List<ICustomFieldKey>();

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    if (field is ICustomFieldKey customField) customFields.Add(customField);
                    else simpleFields.Add(field);
                }
            }
        }

        protected virtual IEnumerable<IIssueProjectionSpecification> LoadSimpleFieldProjection(IEnumerable<FieldKey> fields)
        {
            if (fields != null)
            {
                var queryFields = fields.ToArray();

                foreach (var field in fields)
                {
                    if (fieldProjectionMap.TryGetValue(field, out var simpleFieldProjection))
                    {
                        if (simpleFieldProjection is IIssueProjectionSpecificationFieldKeyDependency fieldKeyDependencyProjection) yield return fieldKeyDependencyProjection.ConstructFrom(field, queryFields);
                        else yield return simpleFieldProjection;
                    }
                    else throw new NotSupportedException($"{field.Name} is not supported by injected projection specification");
                }
            }
        }

        protected virtual Dictionary<ICustomFieldKey, IIssueCustomFieldProjectionSpecification> LoadCustomFieldProjection(IEnumerable<ICustomFieldKey> fields)
        {
            var result = new Dictionary<ICustomFieldKey, IIssueCustomFieldProjectionSpecification>();

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    var projectionHandlerFound = false;
                    foreach (var customFieldProjectionSpec in customFieldProjectionSpecifications)
                    {
                        if (customFieldProjectionSpec.IsSupported(field))
                        {
                            if (customFieldProjectionSpec is IIssueCustomFieldProjectionSpecificationFieldKeyDependency fieldKeyDependencyProjection) result.TryAdd(field, fieldKeyDependencyProjection.ConstructFrom(field));
                            else result.TryAdd(field, customFieldProjectionSpec);

                            projectionHandlerFound = true;
                            break;
                        }
                    }
                    if (projectionHandlerFound == false) throw new NotSupportedException($"{field.Name} is not supported by injected projection specification");
                }
            }
            return result;
        }
    }

    public static class IssueSearchSpecificationExtension
    {
        public static IQuerySpecification[] BuildOptimize(IEnumerable<IQuerySpecification> specs, Dictionary<Type, IIssueQuerySpecificationHandler> querySpecificationHandlerMap)
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

    public class IssueProjection
    {
        public IIssueProjectionSpecification<jiraissue>[] IssueSimpleProjections { get; private init; }
        public IIssueProjectionWithContextSpecification<jiraissue>[] IssueContextProjections { get; private init; }
        public IIssueExternalProjectionSpecification[] IssueExternalProjections { get; private init; }

        public static IssueProjection InitialFrom(IIssueProjectionSpecification[] fields)
        {
            if (fields == null || fields.Length == 0) throw new ArgumentNullException(nameof(fields));

            var issueSimpleProjections = new List<IIssueProjectionSpecification<jiraissue>>();
            var issueContextProjections = new List<IIssueProjectionWithContextSpecification<jiraissue>>();
            var issueExternalProjections = new List<IIssueExternalProjectionSpecification>();

            foreach ( var field in fields) 
            {
                switch (field)
                {
                    case IIssueProjectionSpecification<jiraissue> issueSimpleProjection:
                        issueSimpleProjections.Add(issueSimpleProjection);
                        break;
                    case IIssueProjectionWithContextSpecification<jiraissue> issueContextProjection:
                        issueContextProjections.Add(issueContextProjection);
                        break;
                    case IIssueExternalProjectionSpecification issueExternalProjection:
                        issueExternalProjections.Add(issueExternalProjection);
                        break;
                    default:
                        throw new NotSupportedException($"{field.GetType().Name} is not supported yet");
                }
            }

            return new IssueProjection
            {
                IssueSimpleProjections = issueSimpleProjections.ToArray(),
                IssueContextProjections = issueContextProjections.ToArray(),
                IssueExternalProjections = issueExternalProjections.ToArray()
            };
        }
    }

    public static class IssueProjectionExtension
    {
        public static IQueryable<jiraissue> ProjectionInclude(this IQueryable<jiraissue> inputQuery, IssueProjection issueProjection)
        {
            var projectionIncludeSpecifications = issueProjection.IssueSimpleProjections.Concat<IProjectionIncludeSpecification<jiraissue>>(issueProjection.IssueContextProjections);
            return inputQuery.ProjectionInclude(projectionIncludeSpecifications);
        }
    }
}
