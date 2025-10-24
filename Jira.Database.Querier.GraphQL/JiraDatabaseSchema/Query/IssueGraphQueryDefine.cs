using GraphQL;
using GraphQL.Types;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.Query
{
    public class IssueGraphQueryDefine : IJiraDatabaseGraphQueryDefine
    {
        private readonly IIssueFieldKeyResolver issueFieldKeyResolver;
        private readonly IIssueCustomFieldSchemaTypeMapping issueCustomFieldSchemaTypeMapping;

        public IssueGraphQueryDefine(IIssueFieldKeyResolver issueFieldKeyResolver
            , IIssueCustomFieldSchemaTypeMapping issueCustomFieldSchemaTypeMapping)
        {
            this.issueFieldKeyResolver = issueFieldKeyResolver;
            this.issueCustomFieldSchemaTypeMapping = issueCustomFieldSchemaTypeMapping;
        }

        public void AddQueryField(JiraDatabaseQuery query, JiraDatabaseQuerierBuilder jiraDatabaseQuerierBuilder, JiraDatabaseQuerierBuilder.JiraContextGetterDelegate jiraContextGetter, ILogger logger)
        {
            var issueQueryName = "issue";
            var issueKeyArgumentName = "key";
            var issueIdArgumentName = "id";

            query.Field<IssueGraphType>(issueQueryName)
                .Argument<string>(issueKeyArgumentName, nullable: true)
                .Argument<decimal?>(issueIdArgumentName, nullable: true)
                .ResolveAsync(async context =>
                {
                    var fragmentDefines = context.LoadFragmentDefines().ToArray();
                    var fieldKeySelections = issueFieldKeyResolver.Resolve(context.SubFields, fragmentDefines).ToArray();

                    if (fieldKeySelections.Any())
                    {
                        var issueId = context.GetArgument<decimal?>(issueIdArgumentName);
                        if (issueId.HasValue)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.Issue.GetIssueAsync(issueId.Value, fieldKeySelections);
                            }
                        }

                        var issueKey = context.GetArgument<string>(issueKeyArgumentName);
                        if (string.IsNullOrEmpty(issueKey) == false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.Issue.GetIssueAsync(issueKey, fieldKeySelections);
                            }
                        }

                        return null;
                    }
                    else return null;
                });

            var issuesQueryName = "issues";
            var issuesKeyArgumentName = "keys";
            var issuesIdArgumentName = "ids";

            query.Field<ListGraphType<IssueGraphType>>(issuesQueryName)
                .Argument<string>(issuesKeyArgumentName, nullable: true)
                .Argument<decimal?>(issuesIdArgumentName, nullable: true)
                .ResolveAsync(async context =>
                {
                    var fragmentDefines = context.LoadFragmentDefines().ToArray();
                    var fieldKeySelections = issueFieldKeyResolver.Resolve(context.SubFields, fragmentDefines).ToArray();

                    if (fieldKeySelections.Any())
                    {
                        var issueIds = context.GetArgument<decimal[]>(issuesIdArgumentName);
                        if (issueIds?.Any() ?? false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.Issue.GetIssuesAsync(issueIds, fieldKeySelections);
                            }
                        }

                        var issueKeys = context.GetArgument<string[]>(issuesKeyArgumentName);
                        if (issueKeys?.Any() ?? false)
                        {
                            using (var jiraDatabaseQuerier = jiraDatabaseQuerierBuilder.Build(jiraContextGetter, logger))
                            {
                                return await jiraDatabaseQuerier.Issue.GetIssuesAsync(issueKeys, fieldKeySelections);
                            }
                        }

                        return null;
                    }
                    else return null;
                });
        }

        public void AddSchemaTypeMapping(JiraDatabaseSchema schema)
        {
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueSecurityLevel, IssueSecurityLevelGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueSecurityLevelScheme, IssueSecurityLevelSchemeGraphType>();

            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueStatus, IssueStatusGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueStatusCategory, IssueStatusCategoryGraphType>();

            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssuePriority, IssuePriorityGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueResolution, IssueResolutionGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueType, IssueTypeGraphType>();
            schema.RegisterTypeMapping<Querier.Project.Fields.IProjectComponent, ProjectComponentGraphType>();
            schema.RegisterTypeMapping<Querier.Project.Fields.IProjectVersion, ProjectVersionGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueComment, IssueCommentGraphType>();

            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueWorklog, IssueWorklogGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueChangelog, IssueChangelogGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueChangelogItem, IssueChangelogItemGraphType>();

            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueLink, IssueLinkGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueLinkType, IssueLinkTypeGraphType>();

            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueRemoteLink, IssueRemoteLinkGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.IIssueAttachment, IssueAttachmentGraphType>();

            issueCustomFieldSchemaTypeMapping?.AddSchemaTypeMapping(schema);
        }
    }

    public interface IIssueCustomFieldSchemaTypeMapping
    {
        void AddSchemaTypeMapping(JiraDatabaseSchema schema);
    }
}
