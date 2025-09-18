using lazyzu.Jira.Database.Querier.User;
using lazyzu.Jira.Database.Querier.User.Contract;
using lazyzu.Jira.Database.Querier.User.Fields;
using lazyzu.Jira.Database.Querier.User.Fields.QuerySpecificationHandler;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier
{
    public partial class JiraDatabaseQuerierBuilder
    {
        protected Func<Uri, ILogger, IUserAvatarUrlBuilder> UserAvatarUrlBuilderConstructor;
        protected User.Contract.FieldKey[] DefaultUserQueryFields;
        protected BuildSpecificationDelegate<IUserProjectionSpecification> UserProjectionBuilder;
        protected BuildSpecificationDelegate<IUserQuerySpecificationHandler> UserQuerySpecificationHandlerBuilder;

        public void UseUserAvatarUrlBuilder(Func<Uri, ILogger, IUserAvatarUrlBuilder> build)
            => this.UserAvatarUrlBuilderConstructor = build;

        public void UseDefaultUserQueryField(Func<IEnumerable<User.Contract.FieldKey>> build)
        {
            DefaultUserQueryFields = build().ToArray();
        }

        public void UseUserProjection(bool withDefault = true, BuildSpecificationDelegate<IUserProjectionSpecification> buildExternal = null)
        {
            this.UserProjectionBuilder = (ServiceBuildContext context) =>
            {
                return useUserProjection(context, withDefault, buildExternal);
            };
        }

        protected virtual IEnumerable<IUserProjectionSpecification> useUserProjection(ServiceBuildContext context
            , bool withDefault
            , BuildSpecificationDelegate<IUserProjectionSpecification> buildExternal)
        {
            if (withDefault)
            {
                yield return new UserAppIdProjection();
                yield return new UserCwdIdProjection();
                yield return new UserKeyProjection();
                yield return new UserNameProjection();
                yield return new UserDisplayNameProjection();
                yield return new UserEmailProjection();
                yield return new UserActiveProjection();
                yield return new UserAvatarProjection(context.JiraContext, context.UserAvatarUrlBuilder, context.Logger);
                yield return new UserGroupProjection(context.JiraContext, context.Logger);
            }

            if (buildExternal != null)
            {
                foreach (var externalUserProjection in buildExternal(context))
                {
                    yield return externalUserProjection;
                }
            }
        }

        public void UseUserQuerySpecificationHandler(bool withDefault = true, BuildSpecificationDelegate<IUserQuerySpecificationHandler> buildExternal = null)
        {
            this.UserQuerySpecificationHandlerBuilder = (ServiceBuildContext context)
                => useUserQuerySpecificationHandler(context
                , withDefault
                , buildExternal);
        }

        protected virtual IEnumerable<IUserQuerySpecificationHandler> useUserQuerySpecificationHandler(ServiceBuildContext context
            , bool withDefault
            , BuildSpecificationDelegate<IUserQuerySpecificationHandler> buildExternal)
        {
            if (withDefault)
            {
                yield return new CwdUserQuerySpecificationHandler();
            }

            if (buildExternal != null)
            {
                foreach (var externalProjectProjection in buildExternal(context))
                {
                    yield return externalProjectProjection;
                }
            }
        }

        internal IUserService BuildUserService(ServiceBuildContext context)
        {
            var userProjections = UserProjectionBuilder?.Invoke(context) 
                ?? useUserProjection(context
                                   , withDefault: true
                                   , buildExternal: null);

            var querySpecificationHandlers = UserQuerySpecificationHandlerBuilder?.Invoke(context)
                ?? useUserQuerySpecificationHandler(context, withDefault: true, buildExternal: null);

            var defaultUserQueryFields = DefaultUserQueryFields ?? new FieldKey[]
            {
                UserFieldSelection.UserName,
                UserFieldSelection.UserKey
            };

            return new UserService(context.JiraContext
                , userProjections.ToArray()
                , querySpecificationHandlers.ToArray()
                , defaultUserQueryFields
                , new UserSpecs()
                , context.Logger);
        }
    }
}
