using GraphQL;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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

    public static class JiraDatabaseGraphQueryDefineUtil
    {
        public static IEnumerable<GraphQLParser.AST.GraphQLFragmentDefinition> LoadFragmentDefines(this IResolveFieldContext context)
        {
            foreach (var define in context.Document.Definitions)
            {
                if (define is GraphQLParser.AST.GraphQLFragmentDefinition fragmentDefine) yield return fragmentDefine;
            }
        }
    }
}
