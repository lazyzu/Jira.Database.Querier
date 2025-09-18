using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.User.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.User.Fields
{
    public interface IUserGroup
    {
        decimal Id { get; }
        string Name { get; }
    }

    [Equatable(Explicit = true)]
    public partial class UserGroup : IUserGroup
    {
        [DefaultEquality]
        public decimal Id { get; init; }
        public string Name { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name}";
        }
    }

    internal class UserGroupProjection : IUserProjectionWithContextSpecification<cwd_user>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<cwd_user, object>>[] IncludeExpressions { get; protected set; }

        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public UserGroupProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                UserFieldSelection.UserGroup
            };

            IncludeExpressions = new Expression<Func<cwd_user, object>>[]
            {
                cwdUser => cwdUser.lower_user_name,
                cwdUser => cwdUser.directory_id
            };
        }

        public async Task<object> PrepareContext(IEnumerable<cwd_user> enties, CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<string, List<UserGroup>>();

            foreach (var directoryIdGroup in enties.GroupBy(user => user.directory_id))
            {
                var directoryId = directoryIdGroup.Key;
                if (directoryId.HasValue == false) continue;

                var userNames = enties.Select(user => user.lower_user_name).Distinct().ToArray() ?? new string[0];

                if (userNames.Any())
                {
                    var queryContext = await LoadParentGroup(userNames, directoryId.Value, cancellationToken).ConfigureAwait(false);

                    while (queryContext.QueryGroupNameToUserNameMap.Any())
                    {
                        queryContext = await LoadParentGroup(queryContext, directoryId.Value, cancellationToken).ConfigureAwait(false);
                    }

                    foreach(var userGroupResult in queryContext.ResultCache) result.TryAdd(userGroupResult.Key.ToLower(), userGroupResult.Value);
                }
            }
            return result;
        }

        public Task Projection(cwd_user entity, JiraUser projection, object context, CancellationToken cancellationToken = default)
        {
            if (context is IDictionary<string, List<UserGroup>> resultCache)
            {
                if (resultCache.TryGetValue(entity.lower_user_name, out var groups))
                {
                    projection.Groups = groups.ToHashSet<IUserGroup>();
                }
                else projection.Groups = new HashSet<IUserGroup>();

                return Task.CompletedTask;
            }
            else throw new NotSupportedException();
        }

        protected class QueryContext
        {
            public Dictionary<string, List<UserGroup>> ResultCache { get; init; }
            public Dictionary<string, string[]> QueryGroupNameToUserNameMap { get; init; }
        }

        protected async Task<QueryContext> LoadParentGroup(string[] userNames
            , decimal directoryId
            , CancellationToken cancellationToken = default)
        {
            var query = from cwd_membership in jiraContext.cwd_membership.AsNoTracking()
                        where userNames.Contains(cwd_membership.lower_child_name) 
                           && cwd_membership.directory_id == directoryId
                           && cwd_membership.membership_type == "GROUP_USER"
                        select new
                        {
                            cwd_membership.parent_id,
                            cwd_membership.child_name,
                            cwd_membership.parent_name
                        };

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return new QueryContext
            {
                ResultCache = queryResult.GroupBy(dbModel => dbModel.child_name)
                .ToDictionary(childIdGroup => childIdGroup.Key
                            , childIdGroup => childIdGroup.Select(dbModel => new UserGroup
                            {
                                Id = dbModel.parent_id.Value,
                                Name = dbModel.parent_name,
                            }).ToList()),
                QueryGroupNameToUserNameMap = queryResult.GroupBy(dbModel => dbModel.parent_name)
                .ToDictionary(parentIdGroup => parentIdGroup.Key,
                              parentIdGroup => parentIdGroup.Select(dbModel => dbModel.child_name).ToArray())
            };
        }

        protected async Task<QueryContext> LoadParentGroup(QueryContext context
            , decimal directoryId
            , CancellationToken cancellationToken = default)
        {
            var groupNames = context.QueryGroupNameToUserNameMap.Keys.ToArray();

            var query = from cwd_membership in jiraContext.cwd_membership.AsNoTracking()
                        where groupNames.Contains(cwd_membership.child_name)
                           && cwd_membership.directory_id == directoryId
                           && cwd_membership.membership_type == "GROUP_GROUP"
                        select new
                        {
                            cwd_membership.parent_id,
                            cwd_membership.child_name,
                            cwd_membership.parent_name
                        };

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            if (queryResult.Any())
            {
                var groupParentInfos = queryResult.GroupBy(dbModel => dbModel.child_name)
                .ToDictionary(childIdGroup => childIdGroup.Key
                            , childIdGroup => childIdGroup.Select(dbModel => new UserGroup
                            {
                                Id = dbModel.parent_id.Value,
                                Name = dbModel.parent_name,
                            }).ToList());

                foreach (var groupParentInfo in groupParentInfos)
                {
                    if (context.QueryGroupNameToUserNameMap.TryGetValue(groupParentInfo.Key, out var userIds))
                    {
                        foreach (var userId in userIds)
                        {
                            if (context.ResultCache.TryGetValue(userId, out var groups)) groups.AddRange(groupParentInfo.Value);
                        }
                    }
                }

                return new QueryContext
                {
                    ResultCache = context.ResultCache,
                    QueryGroupNameToUserNameMap = queryResult.GroupBy(dbModel => dbModel.parent_name)
                    .ToDictionary(parentNameGroup => parentNameGroup.Key,
                                  parentNameGroup =>
                                  {
                                      return parentNameGroup.SelectMany(dbModel =>
                                      {
                                          if (context.QueryGroupNameToUserNameMap.TryGetValue(dbModel.child_name, out var userNames)) return userNames;
                                          else return new string[0];
                                      }).Distinct()
                                        .ToArray();
                                  })
                };
            }
            else return new QueryContext
            {
                ResultCache = context.ResultCache,
                QueryGroupNameToUserNameMap = new Dictionary<string, string[]>()
            };
        }
    }
}
