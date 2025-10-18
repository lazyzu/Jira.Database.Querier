using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.Query;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema
{
    public class JiraDatabaseSchema : Schema
    {
        public JiraDatabaseSchema(JiraDatabaseQuery jiraDatabaseQuery
            , IEnumerable<IJiraDatabaseGraphQueryDefine> graphQueryDefines)
        {
            Query = jiraDatabaseQuery;

            foreach (var graphQueryDefine in graphQueryDefines)
            {
                graphQueryDefine.AddSchemaTypeMapping(this);
            }
        }
    }

    public class JiraDatabaseQuery : ObjectGraphType
    {
        private readonly JiraDatabaseQuerierBuilder jiraDatabaseQuerierBuilder;
        private readonly JiraDatabaseQuerierBuilder.JiraContextGetterDelegate jiraContextGetter;
        private readonly ILogger logger;

        public JiraDatabaseQuery(JiraDatabaseQuerierBuilder jiraDatabaseQuerierBuilder
            , JiraDatabaseQuerierBuilder.JiraContextGetterDelegate jiraContextGetter
            , IEnumerable<IJiraDatabaseGraphQueryDefine> graphQueryDefines
            , ILogger logger = null)
        {
            this.jiraDatabaseQuerierBuilder = jiraDatabaseQuerierBuilder;
            this.jiraContextGetter = jiraContextGetter;
            this.logger = logger;

            foreach (var graphQueryDefine in graphQueryDefines)
            {
                graphQueryDefine.AddQueryField(this, jiraDatabaseQuerierBuilder, jiraContextGetter, logger);
            }
        }
    }

    public static class JiraDatabaseCompositionRootHelper
    {
        public static void AddJiraDatabaseSchema(this WebApplicationBuilder applicationBuilder
            , string jiraConnectionString
            , Uri jiraWebUri)
        {
            var services = applicationBuilder.Services;
            services.TryAddSingleton<JiraDatabaseQuerierBuilder>(serviceProvider =>
            {
                var uri = jiraWebUri;
                return new JiraDatabaseQuerierBuilder(uri);
            });
            services.TryAddSingleton<JiraDatabaseQuerierBuilder.JiraContextGetterDelegate>(serviceProvider =>
            {
                return () =>
                {
                    return new Jira.Database.EntityFrameworkCore.MySQL.JiraContext(jiraConnectionString);
                };
            });
            services.TryAddSingleton<JiraDatabaseQuery>();
            services.TryAddSingleton<JiraDatabaseSchema>();
        }

        public static void AddDefaultJiraUserQuery(this WebApplicationBuilder applicationBuilder)
        {
            var services = applicationBuilder.Services;

            services.TryAddSingleton<IUserFieldKeyResolver, UserFieldKeyResolvee>();
            services.AddSingleton<IJiraDatabaseGraphQueryDefine, UserGraphQueryDefine>();
        }

        public static void AddDefaultJiraProjectQuery(this WebApplicationBuilder applicationBuilder)
        {
            var services = applicationBuilder.Services;

            services.TryAddSingleton<IProjectFieldKeyResolver, ProjectFieldKeyResolver>();
            services.AddSingleton<IJiraDatabaseGraphQueryDefine, ProjectGraphQueryDefine>();
        }

        public static void AddDefaultJiraIssueQuery(this WebApplicationBuilder applicationBuilder, IEnumerable<ICustomFieldSource> customFieldSources)
        {
            var services = applicationBuilder.Services;

            services.TryAddSingleton<IIssueFieldKeyResolver, IssueFieldKeyResolver>();

            // Add custom fields to issue
            IssueGraphType.CustomFieldHandler = new IssueGraphField_CustomFieldHandler();
            if (customFieldSources?.Any() ?? false)
            {
                IssueGraphType.CustomFieldSources.AddRange(customFieldSources);
            }

            services.AddSingleton<IIssueCustomFieldSchemaTypeMapping, IssueCustomFieldSchemaTypeMapping>();

            services.AddSingleton<IJiraDatabaseGraphQueryDefine, IssueGraphQueryDefine>();
        }
    }
}
