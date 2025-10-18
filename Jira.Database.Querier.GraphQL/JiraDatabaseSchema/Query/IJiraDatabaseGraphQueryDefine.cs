using Microsoft.Extensions.Logging;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.Query
{
    public interface IJiraDatabaseGraphQueryDefine
    {
        void AddQueryField(JiraDatabaseQuery query
            , JiraDatabaseQuerierBuilder jiraDatabaseQuerierBuilder
            , JiraDatabaseQuerierBuilder.JiraContextGetterDelegate jiraContextGetter
            , ILogger logger);

        void AddSchemaTypeMapping(JiraDatabaseSchema schema);
    }
}
