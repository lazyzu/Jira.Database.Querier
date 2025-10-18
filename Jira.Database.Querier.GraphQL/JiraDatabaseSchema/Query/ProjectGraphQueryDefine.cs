using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Shared;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.User;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.Query
{
    public class ProjectGraphQueryDefine : IJiraDatabaseGraphQueryDefine
    {
        private readonly IProjectFieldKeyResolver projectFieldKeyResolver;

        public ProjectGraphQueryDefine(IProjectFieldKeyResolver projectFieldKeyResolver)
        {
            this.projectFieldKeyResolver = projectFieldKeyResolver;
        }

        public void AddQueryField(JiraDatabaseQuery query, JiraDatabaseQuerierBuilder jiraDatabaseQuerierBuilder, JiraDatabaseQuerierBuilder.JiraContextGetterDelegate jiraContextGetter, ILogger logger)
        {
            var projectQueryName = "project";
            var projectKeyArgumentName = "key";
            var projectIdArgumentName = "id";

            query.Field<ProjectGraphType>(projectQueryName)
                .Argument<string>(projectKeyArgumentName, nullable: true)
                .Argument<decimal?>(projectIdArgumentName, nullable: true)
                .ResolveAsync(async context =>
                {
                    var fieldKeySelections = projectFieldKeyResolver.Resolve(context.SubFields).ToArray();

                    if (fieldKeySelections.Any())
                    {
                        var projectId = context.GetArgument<decimal?>(projectIdArgumentName);
                        if (projectId.HasValue)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.Project.GetProjectAsync(projectId.Value, fieldKeySelections);
                            }
                        }

                        var projectKey = context.GetArgument<string>(projectKeyArgumentName);
                        if (string.IsNullOrEmpty(projectKey) == false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.Project.GetProjectAsync(projectKey, fieldKeySelections);
                            }
                        }

                        return null;
                    }
                    else return null;
                });

            var projectsQueryName = "projects";
            var projectsKeyArgumentName = "keys";
            var projectsIdArgumentName = "ids";

            query.Field<ListGraphType<ProjectGraphType>>(projectsQueryName)
                .Argument<string[]>(projectsKeyArgumentName, nullable: true)
                .Argument<decimal[]>(projectsIdArgumentName, nullable: true)
                .ResolveAsync(async context =>
                {
                    var fieldKeySelections = projectFieldKeyResolver.Resolve(context.SubFields).ToArray();

                    if (fieldKeySelections.Any())
                    {
                        var projectIds = context.GetArgument<decimal[]>(projectsIdArgumentName);
                        if (projectIds?.Any() ?? false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.Project.GetProjectsAsync(projectIds, fieldKeySelections);
                            }
                        }

                        var projectKeys = context.GetArgument<string[]>(projectsKeyArgumentName);
                        if (projectKeys?.Any() ?? false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.Project.GetProjectsAsync(projectKeys, fieldKeySelections);
                            }
                        }
                        
                        return new Project.IJiraProject[0];
                    }
                    else return null;
                });
        }

        public void AddSchemaTypeMapping(JiraDatabaseSchema schema)
        {
            schema.RegisterTypeMapping(typeof(Querier.User.IJiraUser), typeof(UserGraphType));

            schema.RegisterTypeMapping(typeof(Querier.Project.Fields.IProjectAvatar), typeof(ProjectAvatarGraphType));
            schema.RegisterTypeMapping(typeof(Querier.Avatar.IAvatarUrl), typeof(AvatarUrlGraphType));
            schema.RegisterTypeMapping(typeof(Querier.Project.Fields.IProjectCategory), typeof(ProjectCategoryGraphType));
            schema.RegisterTypeMapping(typeof(Querier.Project.Fields.IProjectRoleActorMap), typeof(ProjectRoleActorMapGraphType));
            schema.RegisterTypeMapping(typeof(Querier.Project.Fields.IProjectRoleActor), typeof(ProjectRoleActorGraphType));

            schema.RegisterTypeMapping(typeof(Querier.Project.Fields.IIssueTypeScheme), typeof(IssueTypeSchemeGraphType));
            schema.RegisterTypeMapping(typeof(Querier.Issue.Fields.IIssueType), typeof(IssueTypeGraphType));

            schema.RegisterTypeMapping(typeof(Querier.Project.Fields.IFullProjectComponent), typeof(FullProjectComponentGraphType));

            schema.RegisterTypeMapping(typeof(Querier.Project.Fields.IFullProjectVersion), typeof(FullProjectVersionGraphType));

            schema.RegisterTypeMapping(typeof(Querier.Issue.Fields.IIssueSecurityLevel), typeof(IssueSecurityLevelGraphType));
            schema.RegisterTypeMapping(typeof(Querier.Issue.Fields.IIssueSecurityLevelScheme), typeof(IssueSecurityLevelSchemeGraphType));
        }
    }
}
