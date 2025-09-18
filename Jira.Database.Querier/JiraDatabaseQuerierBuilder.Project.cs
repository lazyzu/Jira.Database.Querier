using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.Project.Contract;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.Project.Fields.QuerySpecificationHandler;
using lazyzu.Jira.Database.Querier.Project.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier
{
    public partial class JiraDatabaseQuerierBuilder
    {
        protected BuildServiceDelegate<IProjectKeyService> ProjectKeyServiceBuilder;
        protected BuildServiceDelegate<IProjectCategoryService> ProjectCategoryServiceBuilder;
        protected BuildServiceDelegate<IProjectRoleService> ProjectRoleServiceBuilder;
        protected Func<Uri, ILogger, IProjectAvatarUrlBuilder> ProjectAvatarUrlBuilderConstructor;
        protected Project.Contract.FieldKey[] DefaultProjectQueryFields;
        protected BuildSpecificationDelegate<IProjectProjectionSpecification> ProjectProjectionBuilder;
        protected BuildSpecificationDelegate<IProjectQuerySpecificationHandler> ProjectQuerySpecificationHandlerBuilder;

        public void UseProjectKeyService(BuildServiceDelegate<IProjectKeyService> build)
        {
            this.ProjectKeyServiceBuilder = build;
        }

        public void UseProjectCategoryService(BuildServiceDelegate<IProjectCategoryService> build)
        {
            this.ProjectCategoryServiceBuilder = build;
        }

        public void UseProjectRoleService(BuildServiceDelegate<IProjectRoleService> build)
        {
            this.ProjectRoleServiceBuilder = build;
        }

        public void UseProjectAvatarUrlBuilder(Func<Uri, ILogger, IProjectAvatarUrlBuilder> build)
        {
            this.ProjectAvatarUrlBuilderConstructor = build;
        }

        public void UseDefaultProjectQueryField(Func<IEnumerable<Project.Contract.FieldKey>> build)
        {
            DefaultProjectQueryFields = build().ToArray();
        }

        internal class ProjectFieldServiceCollection : IProjectFieldServiceCollection
        {
            public IProjectKeyService ProjectKey { get; init; }
            public IProjectCategoryService ProjectCategory { get; init; }
            public IProjectRoleService ProjectRole { get; init; }
        }

        public void UseProjectProjection(bool withDefault = true, BuildSpecificationDelegate<IProjectProjectionSpecification> buildExternal = null)
        {
            this.ProjectProjectionBuilder = (ServiceBuildContext context) =>
            {
                return useProjectProjection(context, withDefault, buildExternal);
            };
        }

        protected virtual IEnumerable<IProjectProjectionSpecification> useProjectProjection(ServiceBuildContext context
            , bool withDefault
            , BuildSpecificationDelegate<IProjectProjectionSpecification> buildExternal)
        {
            if (withDefault)
            {
                yield return new ProjectNameProjection();
                yield return new ProjectUrlProjection();
                yield return new ProjectLeadProjection(context.JiraDatabaseQuerierGetter);
                yield return new ProjectDescriptionProjection();
                yield return new ProjectKeyProjection();
                yield return new ProjectAvatarProjection(context.ProjectAvatarUrlBuilder);
                yield return new ProjectTypeProjection();
                yield return new ProjectCategoryProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Cache, context.Logger);
                yield return new ProjectRoleProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Cache, context.Logger);
                yield return new ProjectIssueTypeProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Cache, context.Logger);
                yield return new ProjectComponentProjection(context.JiraContext, context.Cache, context.Logger);
                yield return new ProjectVersionProjection(context.JiraContext, context.Cache, context.Logger);
                yield return new ProjectIssueSecurityLevelProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Cache, context.Logger);
            }

            if (buildExternal != null)
            {
                foreach (var externalProjectProjection in buildExternal(context))
                {
                    yield return externalProjectProjection;
                }
            }
        }

        public void UseProjectQuerySpecificationHandler(bool withDefault = true, BuildSpecificationDelegate<IProjectQuerySpecificationHandler> buildExternal = null)
        {
            this.ProjectQuerySpecificationHandlerBuilder = (ServiceBuildContext context)
                 => useProjectQuerySpecificationHandler(context
                , withDefault
                , buildExternal);
        }

        protected virtual IEnumerable<IProjectQuerySpecificationHandler> useProjectQuerySpecificationHandler(ServiceBuildContext context
            , bool withDefault
            , BuildSpecificationDelegate<IProjectQuerySpecificationHandler> buildExternal)
        {
            if (withDefault)
            {
                yield return new ProjectQuerySpecificationHandler();
            }

            if (buildExternal != null)
            {
                foreach (var externalProjectProjection in buildExternal(context))
                {
                    yield return externalProjectProjection;
                }
            }
        }

        internal IProjectFieldServiceCollection BuildProjectFieldServiceCollection(ServiceBuildContext context)
        {
            var projectKeyService = ProjectKeyServiceBuilder?.Invoke(context)
                ?? new ProjectKeyService(context.JiraContext, context.Cache, context.Logger);

            var projectCategoryService = ProjectCategoryServiceBuilder?.Invoke(context)
                ?? new ProjectCategoryService(context.JiraContext, context.Cache, context.Logger);

            var projectRoleService = ProjectRoleServiceBuilder?.Invoke(context)
                ?? new ProjectRoleService(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Cache, context.Logger);

            return new ProjectFieldServiceCollection
            {
                ProjectKey = projectKeyService,
                ProjectCategory = projectCategoryService,
                ProjectRole = projectRoleService,
            };
        }

        internal IProjectService BuildProjectService(ServiceBuildContext context, IProjectFieldServiceCollection projectFieldServiceCollection)
        {
            var projectProjections = ProjectProjectionBuilder?.Invoke(context)
                ?? useProjectProjection(context
                                      , withDefault: true
                                      , buildExternal: null);

            var querySpecificationHandlers = ProjectQuerySpecificationHandlerBuilder?.Invoke(context)
                ?? useProjectQuerySpecificationHandler(context, withDefault: true, buildExternal: null);

            var defaultProjectQueryFields = DefaultProjectQueryFields ?? new FieldKey[]
            {
                ProjectFieldSelection.ProjectKey,
                ProjectFieldSelection.ProjectName
            };

            return new ProjectService(context.JiraContext
                , projectProjections.ToArray()
                , querySpecificationHandlers.ToArray()
                , defaultProjectQueryFields
                , new ProjectSpecs()
                , context.Logger)
            {
                ProjectKey = projectFieldServiceCollection.ProjectKey,
                ProjectCategory = projectFieldServiceCollection.ProjectCategory,
                ProjectRole = projectFieldServiceCollection.ProjectRole
            };
        }
    }
}
