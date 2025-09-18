using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.ProjectionSpecification;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using lazyzu.Jira.Database.Querier.User.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.User
{
    public interface IUserService
    {
        ImmutableArray<FieldKey> DefaultQueryFields { get; }

        Task<IJiraUser> GetUserByKeyAsync(string userKey, FieldKey[] fields = null, CancellationToken cancellationToken = default);
        Task<IJiraUser> GetUserByNameAsync(string userName, FieldKey[] fields = null, CancellationToken cancellationToken = default);
        Task<IJiraUser[]> GetUsersByKeyAsync(IEnumerable<string> userKeys, FieldKey[] fields = null, CancellationToken cancellationToken = default);
        Task<IJiraUser[]> GetUsersByNameAsync(IEnumerable<string> userNames, FieldKey[] fields = null, CancellationToken cancellationToken = default);
        Task<IJiraUser[]> SearchUserAsync(Func<IUserSpecs, IQuerySpecification[]> userSearchSpecificationBuilder, FieldKey[] fields = null, CancellationToken cancellationToken = default);
    }

    public class UserService : IUserService
    {
        public ImmutableArray<FieldKey> DefaultQueryFields { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly IUserProjectionSpecification[] userProjectionSpecifications;
        protected readonly Dictionary<FieldKey, IUserProjectionSpecification> userProjectionMap;
        protected readonly IUserQuerySpecificationHandler[] userQuerySpecificationHandlers;
        protected readonly Dictionary<Type, IUserQuerySpecificationHandler> querySpecificationHandlerMap;
        protected readonly IUserSpecs userSpecs;
        protected readonly ILogger logger;

        public UserService(JiraContext jiraContext
            , IUserProjectionSpecification[] userProjectionSpecifications
            , IUserQuerySpecificationHandler[] userQuerySpecificationHandlers
            , FieldKey[] defaultQueryFields
            , IUserSpecs userSpecs
            , ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.userProjectionSpecifications = userProjectionSpecifications;
            this.userProjectionMap = ToUserProjectionMap(userProjectionSpecifications);
            this.userQuerySpecificationHandlers = userQuerySpecificationHandlers ?? new IUserQuerySpecificationHandler[0];
            this.querySpecificationHandlerMap = ToQuerySpecificationHandlerMap(this.userQuerySpecificationHandlers);
            this.DefaultQueryFields = defaultQueryFields?.ToImmutableArray() ?? ImmutableArray<FieldKey>.Empty;
            this.userSpecs = userSpecs;
            this.logger = logger;
        }

        protected virtual Dictionary<FieldKey, IUserProjectionSpecification> ToUserProjectionMap(IUserProjectionSpecification[] userProjectionSpecifications)
        {
            var result = new Dictionary<FieldKey, IUserProjectionSpecification>();
            if (userProjectionSpecifications.Any())
            {
                foreach (var userProjectionSpec in userProjectionSpecifications)
                {
                    var handleTargets = userProjectionSpec.HandleTarget.ToArray();
                    foreach (var handleTarget in handleTargets)
                    {
                        result.TryAdd(handleTarget, userProjectionSpec);
                    }
                }
            }
            return result;
        }

        protected virtual Dictionary<Type, IUserQuerySpecificationHandler> ToQuerySpecificationHandlerMap(IUserQuerySpecificationHandler[] userQuerySpecificationHandlers)
        {
            var result = new Dictionary<Type, IUserQuerySpecificationHandler>();
            if (userQuerySpecificationHandlers != null)
            {
                foreach (var querySpecificationHandler in userQuerySpecificationHandlers)
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

        public virtual async Task<IJiraUser[]> SearchUserAsync(Func<IUserSpecs, IQuerySpecification[]> userSearchSpecificationBuilder, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var userSearchSpecifications = userSearchSpecificationBuilder?.Invoke(userSpecs) ?? new IQuerySpecification[0];
            var optimizedUserSearchSpecification = UserSearchSpecificationExtension.BuildOptimize(userSearchSpecifications, querySpecificationHandlerMap);

            if (optimizedUserSearchSpecification.Any())
            {
                string[] targetLowerUserNames = null;
                foreach (var querySpecification in optimizedUserSearchSpecification)
                {
                    if (querySpecificationHandlerMap.TryGetValue(querySpecification.SchemaType, out var querySpecificationHandler))
                    {
                        targetLowerUserNames = await querySpecificationHandler.SearchIssueAsync(jiraContext, querySpecification, targetLowerUserNames, cancellationToken).ConfigureAwait(false);
                    }
                    else throw new NotSupportedException($"{querySpecification.GetType().Name} is not supported issue spec");

                    if (targetLowerUserNames != null && targetLowerUserNames.Length == 0) break;
                }

                return await GetUsersByNameAsync(targetLowerUserNames, fields, cancellationToken).ConfigureAwait(false);
            }
            else return new IJiraUser[0];
        }

        public virtual async Task<IJiraUser[]> GetUsersByKeyAsync(IEnumerable<string> userKeys, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var _userKeys = userKeys?.Select(userKey => userKey?.Trim())
                ?.Where(userName => string.IsNullOrEmpty(userName) == false)
                ?.Distinct()
                ?.ToArray() ?? new string[0];
            if (_userKeys.Any() == false) return new IJiraUser[0];

            var userNameQuery = jiraContext.app_user.AsNoTracking()
                    .Where(appUser => _userKeys.Contains(appUser.user_key))
                    .Select(appUser => appUser.lower_user_name);

            var userNames = await userNameQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return await GetUsersByNameAsync(userNames, fields, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IJiraUser[]> GetUsersByNameAsync(IEnumerable<string> userNames, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var _userNames = userNames?.Select(userName => userName?.Trim()?.ToLower())
                ?.Where(userName => string.IsNullOrEmpty(userName) == false)
                ?.Distinct()
                ?.ToArray() ?? new string[0];

            if (_userNames.Any())
            {
                var _fields = InitFieldSelection(fields);
                var userProjections = LoadUserProjection(_fields).ToArray();
                var userProjectionSelection = UserProjection.InitialFrom(userProjections);

                var userNameProjectionMap = _userNames.ToDictionary(userName => userName, userName => new ProjectionCache
                {
                    Rendered = false,
                    Projection = new JiraUser()
                });

                if (userProjectionSelection.NeedToQueryAppUser())
                {
                    var query = jiraContext.app_user.AsNoTracking()
                        .Where(appUser => _userNames.Contains(appUser.lower_user_name))
                        .ProjectionInclude(userProjectionSelection, new string[]
                        {
                            "lower_user_name"
                        });

                    var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                    foreach (var field in userProjectionSelection.AppSimpleProjections)
                    {
                        foreach (var dbModel in queryResult)
                        {
                            if (userNameProjectionMap.TryGetValue(dbModel.lower_user_name, out var projectionCache))
                            {
                                await field.Projection(dbModel, projectionCache.Projection, cancellationToken).ConfigureAwait(false);
                                projectionCache.Rendered = true;
                            }
                        }
                    }

                    foreach (var field in userProjectionSelection.AppContextProjections)
                    {
                        var fieldRenderContext = await field.PrepareContext(queryResult, cancellationToken);

                        foreach (var dbModel in queryResult)
                        {
                            if (userNameProjectionMap.TryGetValue(dbModel.lower_user_name, out var projectionCache))
                            {
                                await field.Projection(dbModel, projectionCache.Projection, fieldRenderContext, cancellationToken).ConfigureAwait(false);
                                projectionCache.Rendered = true;
                            }
                        }
                    }
                }

                if (userProjectionSelection.NeedToQueryCwdUser())
                {
                    var query = jiraContext.cwd_user.AsNoTracking()
                        .Where(cwdUser => _userNames.Contains(cwdUser.lower_user_name))
                        .ProjectionInclude(userProjectionSelection, new string[]
                        {
                            "lower_user_name"
                        });

                    var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                    foreach (var field in userProjectionSelection.CwdSimpleProjections)
                    {
                        foreach (var dbModel in queryResult)
                        {
                            if (userNameProjectionMap.TryGetValue(dbModel.lower_user_name, out var projectionCache))
                            {
                                await field.Projection(dbModel, projectionCache.Projection, cancellationToken).ConfigureAwait(false);

                            }
                        }
                    }

                    foreach (var field in userProjectionSelection.CwdContextProjections)
                    {
                        var fieldRenderContext = await field.PrepareContext(queryResult, cancellationToken);

                        foreach (var dbModel in queryResult)
                        {
                            if (userNameProjectionMap.TryGetValue(dbModel.lower_user_name, out var projectionCache))
                            {
                                await field.Projection(dbModel, projectionCache.Projection, fieldRenderContext, cancellationToken).ConfigureAwait(false);
                                projectionCache.Rendered = true;
                            }
                        }
                    }
                }

                return userNameProjectionMap.Values
                    .Where(projectionCache => projectionCache.Rendered)
                    .Select(projectionCache => projectionCache.Projection)
                    .ToArray();
            }

            return new IJiraUser[0];
        }

        protected class ProjectionCache
        {
            public bool Rendered { get; set; }
            public JiraUser Projection { get; set; }
        }

        public virtual async Task<IJiraUser> GetUserByNameAsync(string userName, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var _userName = userName?.Trim()?.ToLower();
            if (string.IsNullOrEmpty(_userName)) return default;

            var _fields = InitFieldSelection(fields);
            var userProjections = LoadUserProjection(_fields).ToArray();
            var userProjectionSelection = UserProjection.InitialFrom(userProjections);

            var resultProjectProjection = new JiraUser();

            if (userProjectionSelection.NeedToQueryAppUser())
            {
                var query = jiraContext.app_user.AsNoTracking()
                    .Where(appUser => appUser.lower_user_name == _userName)
                    .ProjectionInclude(userProjectionSelection);

                var queryResult = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (queryResult == default) return default;

                foreach (var field in userProjectionSelection.AppSimpleProjections)
                {
                    await field.Projection(queryResult, resultProjectProjection, cancellationToken).ConfigureAwait(false);
                }

                foreach (var field in userProjectionSelection.AppContextProjections)
                {
                    var fieldRenderContext = await field.PrepareContext(new app_user[]
                    {
                        queryResult
                    }, cancellationToken);

                    await field.Projection(queryResult, resultProjectProjection, fieldRenderContext, cancellationToken).ConfigureAwait(false);
                }
            }

            if (userProjectionSelection.NeedToQueryCwdUser())
            {
                var query = jiraContext.cwd_user.AsNoTracking()
                    .Where(cwdUser => cwdUser.lower_user_name == _userName)
                    .ProjectionInclude(userProjectionSelection);

                var queryResult = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                if (queryResult == default) return default;

                foreach (var field in userProjectionSelection.CwdSimpleProjections)
                {
                    await field.Projection(queryResult, resultProjectProjection, cancellationToken).ConfigureAwait(false);
                }

                foreach (var field in userProjectionSelection.CwdContextProjections)
                {
                    var fieldRenderContext = await field.PrepareContext(new cwd_user[]
                    {
                        queryResult
                    }, cancellationToken);

                    await field.Projection(queryResult, resultProjectProjection, fieldRenderContext, cancellationToken).ConfigureAwait(false);
                }
            }

            return resultProjectProjection;
        }

        public virtual async Task<IJiraUser> GetUserByKeyAsync(string userKey, FieldKey[] fields = null, CancellationToken cancellationToken = default)
        {
            var _userKey = userKey?.Trim();
            if (string.IsNullOrEmpty(_userKey)) return default;

            var userNameQuery = jiraContext.app_user.AsNoTracking()
                    .Where(appUser => appUser.user_key == _userKey)
                    .Select(appUser => appUser.lower_user_name);

            var userName = await userNameQuery.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            if (userName == null) return default;
            else return await GetUserByNameAsync(userName, fields, cancellationToken).ConfigureAwait(false);
        }

        protected virtual ImmutableArray<FieldKey> InitFieldSelection(IEnumerable<FieldKey> fields)
        {
            var _fields = fields?.ToImmutableArray() ?? DefaultQueryFields;
            return _fields;
        }

        protected virtual IEnumerable<IUserProjectionSpecification> LoadUserProjection(ImmutableArray<FieldKey> fields)
        {
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    if (userProjectionMap.TryGetValue(field, out var userProjection)) yield return userProjection;
                    else throw new NotSupportedException($"{field.Name} is not supported by injected projection specification");
                }
            }
        }
    }

    public static class UserSearchSpecificationExtension
    {
        public static IQuerySpecification[] BuildOptimize(IEnumerable<IQuerySpecification> specs, Dictionary<Type, IUserQuerySpecificationHandler> querySpecificationHandlerMap)
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

    public class UserProjection
    {
        public IUserProjectionSpecification<cwd_user>[] CwdSimpleProjections { get; private init; }
        public IUserProjectionWithContextSpecification<cwd_user>[] CwdContextProjections { get; private init; }

        public IUserProjectionSpecification<app_user>[] AppSimpleProjections { get; private init; }
        public IUserProjectionWithContextSpecification<app_user>[] AppContextProjections { get; private init; }

        // public IUserExternalProjectionSpecification[] ExternalProjections { get; private init; }

        public static UserProjection InitialFrom(IUserProjectionSpecification[] projectionSpecifications)
        {
            if (projectionSpecifications == null || projectionSpecifications.Length == 0) throw new ArgumentNullException(nameof(projectionSpecifications));

            var cwdSimpleProjections = new List<IUserProjectionSpecification<cwd_user>>();
            var cwdContextProjections = new List<IUserProjectionWithContextSpecification<cwd_user>>();
            var appSimpleProjections = new List<IUserProjectionSpecification<app_user>>();
            var appContextProjections = new List<IUserProjectionWithContextSpecification<app_user>>();
            //var externalProjections = new List<IUserExternalProjectionSpecification>();

            foreach (var projectionSpecification in projectionSpecifications)
            {
                switch (projectionSpecification)
                {
                    case IUserProjectionSpecification<cwd_user> cwdSimpleProjection:
                        cwdSimpleProjections.Add(cwdSimpleProjection);
                        break;
                    case IUserProjectionWithContextSpecification<cwd_user> cwdContextProjection:
                        cwdContextProjections.Add(cwdContextProjection);
                        break;
                    case IUserProjectionSpecification<app_user> appSimpleProjection:
                        appSimpleProjections.Add(appSimpleProjection);
                        break;
                    case IUserProjectionWithContextSpecification<app_user> appContextProjection:
                        appContextProjections.Add(appContextProjection);
                        break;
                    //case IUserExternalProjectionSpecification externalProjection:
                    //    externalProjections.Add(externalProjection);
                    //    break;
                    default:
                        throw new NotSupportedException($"{projectionSpecification.GetType().Name} is not supported yet");
                }
            }

            return new UserProjection
            {
                CwdSimpleProjections = cwdSimpleProjections.ToArray(),
                CwdContextProjections = cwdContextProjections.ToArray(),
                AppSimpleProjections = appSimpleProjections.ToArray(),
                AppContextProjections = appContextProjections.ToArray(),
                //ExternalProjections = externalProjections.ToArray()
            };
        }

        public bool NeedToQueryCwdUser()
        {
            return CwdSimpleProjections.Any() || CwdContextProjections.Any();
        }

        public bool NeedToQueryAppUser()
        {
            return AppSimpleProjections.Any() || AppContextProjections.Any();
        }
    }

    public static class UserProjectionExtension
    {
        public static IQueryable<cwd_user> ProjectionInclude(this IQueryable<cwd_user> inputQuery, UserProjection projectProjection, string[] additionalFields = null)
        {
            var projectionIncludeSpecifications = projectProjection.CwdSimpleProjections.Concat<IProjectionIncludeSpecification<cwd_user>>(projectProjection.CwdContextProjections);
            return inputQuery.ProjectionInclude(projectionIncludeSpecifications, additionalFields);
        }

        public static IQueryable<app_user> ProjectionInclude(this IQueryable<app_user> inputQuery, UserProjection projectProjection, string[] additionalFields = null)
        {
            var projectionIncludeSpecifications = projectProjection.AppSimpleProjections.Concat<IProjectionIncludeSpecification<app_user>>(projectProjection.AppContextProjections);
            return inputQuery.ProjectionInclude(projectionIncludeSpecifications, additionalFields);
        }
    }
}
