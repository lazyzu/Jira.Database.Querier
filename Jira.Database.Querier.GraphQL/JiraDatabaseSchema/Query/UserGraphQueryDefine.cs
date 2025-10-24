using GraphQL;
using GraphQL.Types;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Shared;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.User;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.Query
{
    public class UserGraphQueryDefine : IJiraDatabaseGraphQueryDefine
    {
        protected readonly IUserFieldKeyResolver userFieldKeyResolve;

        public UserGraphQueryDefine(IUserFieldKeyResolver userFieldKeyResolve)
        {
            this.userFieldKeyResolve = userFieldKeyResolve;
        }

        public void AddQueryField(JiraDatabaseQuery query
            , JiraDatabaseQuerierBuilder jiraDatabaseQuerierBuilder
            , JiraDatabaseQuerierBuilder.JiraContextGetterDelegate jiraContextGetter
            , ILogger logger)
        {
            var userQueryName = "user";
            var userNameArgument = "username";
            var userKeyArgument = "key";

            query.Field<UserGraphType>(userQueryName)
                .Argument<string>(userNameArgument, nullable: true)
                .Argument<string>(userKeyArgument, nullable: true)
                .ResolveAsync(async context =>
                {
                    var fragmentDefines = context.LoadFragmentDefines().ToArray();
                    var fieldKeySelections = userFieldKeyResolve.Resolve(context.SubFields, fragmentDefines).ToArray();

                    if (fieldKeySelections.Any())
                    {
                        var username = context.GetArgument<string>(userNameArgument);
                        if (string.IsNullOrEmpty(username) == false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.User.GetUserByNameAsync(username, fieldKeySelections);
                            }
                        }

                        var userKey = context.GetArgument<string>(userKeyArgument);
                        if (string.IsNullOrEmpty(userKey) == false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.User.GetUserByKeyAsync(userKey, fieldKeySelections);
                            }
                        }

                        return null;
                    }
                    else return null;
                });

            var usersQueryName = "users";
            var usersNameArgument = "usernames";
            var usersKeyArgument = "keys";

            query.Field<ListGraphType<UserGraphType>>(usersQueryName)
                .Argument<string[]>(usersNameArgument, nullable: true)
                .Argument<string[]>(usersKeyArgument, nullable: true)
                .ResolveAsync(async context =>
                {
                    var fragmentDefines = context.LoadFragmentDefines().ToArray();
                    var fieldKeySelections = userFieldKeyResolve.Resolve(context.SubFields, fragmentDefines).ToArray();

                    if (fieldKeySelections.Any())
                    {
                        var usernames = context.GetArgument<string[]>(usersNameArgument);
                        if (usernames?.Any() ?? false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.User.GetUsersByNameAsync(usernames, fieldKeySelections);
                            }
                        }

                        var userKeys = context.GetArgument<string[]>(usersKeyArgument);
                        if (userKeys?.Any() ?? false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.User.GetUsersByKeyAsync(userKeys, fieldKeySelections);
                            }
                        }

                        return null;
                    }
                    else return null;
                });
        }

        public void AddSchemaTypeMapping(JiraDatabaseSchema schema)
        {
            schema.RegisterTypeMapping(typeof(Querier.User.Fields.IUserAvatar), typeof(UserAvatarGraphType));
            schema.RegisterTypeMapping(typeof(Querier.Avatar.IAvatarUrl), typeof(AvatarUrlGraphType));
            schema.RegisterTypeMapping(typeof(Querier.User.Fields.IUserGroup), typeof(UserGroupGraphType));
        }
    }
}
