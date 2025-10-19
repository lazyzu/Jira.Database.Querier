using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue;
using GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

builder.AddJiraDatabaseSchema(jiraConnectionString: "...", jiraWebUri: new Uri("https://jira.ooo.com/"));
builder.AddDefaultJiraUserQuery();
builder.AddDefaultJiraProjectQuery();
builder.AddDefaultJiraIssueQuery(new ICustomFieldSource[]
{
    new OOOCustomFieldSource()
});
builder.Services.AddGraphQL(gqlBuilder =>
{
    if (builder.Environment.IsProduction() == false)
    {
        gqlBuilder = gqlBuilder.AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = true);
    }

    gqlBuilder = gqlBuilder.AddSchema<JiraDatabaseSchema>()
                     .AddSystemTextJson(action: option =>
                     {
                         option.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

                         // var typeInfoResolver = new DefaultJsonTypeInfoResolver { };
                         // typeInfoResolver.Modifiers.Add(ProjectGraphType.ProjectJsonSerializeOptimize);
                         // option.TypeInfoResolver = typeInfoResolver;
                     });
});

var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin()
  .AllowAnyHeader()
  .AllowAnyMethod());

app.UseGraphQL(path: "/graphql");
app.UseGraphQLAltair(path: "/ui/altair");

app.Run();

public class OOOCustomFieldSource : ICustomFieldSource
{
    public IEnumerable<CustomFieldDefine> GetEnumerable()
    {
        yield break;
        // yield return new CustomFieldDefine(nameof(lazyzu.Jira.Database.OOO.Issue.CustomField.SameAsJiraFieldName), lazyzu.Jira.Database.OOO.Issue.CustomField.SameAsJiraFieldName);
    }
}
