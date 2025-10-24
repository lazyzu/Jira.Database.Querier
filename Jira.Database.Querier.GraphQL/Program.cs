using GraphQL;
using lazyzu.Jira.Database.Querier.GraphQL;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

#if InMemorySIM
using var inMemoryFakeContext = await JiraDatabaseQueryConfigureUtil.Configure_InMemorySim(builder);
#else
JiraDatabaseQueryConfigureUtil.ConfigureJiraDatabaseQuery();
#endif

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

app.UseCors(builder => builder
  .AllowAnyOrigin()
  .AllowAnyHeader()
  .AllowAnyMethod());

app.UseGraphQL(path: "/graphql");
app.UseGraphQLAltair(path: "/ui/altair");

app.Run();