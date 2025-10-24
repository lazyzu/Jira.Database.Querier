using lazyzu.Jira.Database.Querier.Fake;
using lazyzu.Jira.Database.Querier.GraphQL.InMemoryFake;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.GraphQL
{
    public static class JiraDatabaseQueryConfigureUtil
    {
        public static async Task<InMemoryFakeContext> Configure_InMemorySim(WebApplicationBuilder builder)
        {
            var inMemoryFakeContext = new InMemoryFakeContext();

            var referenceUsers = await inMemoryFakeContext.GenerateUsers(5);
            var referenceProjects = await inMemoryFakeContext.GenerateProjects(5, new InMemoryFakeContext.ProjectGenerateArgument(referenceUsers));
            var cascadingSelectFieldKey = CustomFieldKeyFake.BuildCustomFieldKey<CascadingSelectCustomFieldSchema>("Cascading Select");

            var generatedIssue = await inMemoryFakeContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
            {
                RenderCustomField = (customFields, issue, faker) =>
                {
                    customFields.Add(cascadingSelectFieldKey, new CascadingSelectCustomFieldSchema
                    {
                        Value = new CascadingSelection
                        {
                            CascadingSelections = new ISelectOption[]
                            {
                    CustomFieldKeyFake.BuildOption("cascading-select:1"),
                    CustomFieldKeyFake.BuildOption("cascading-select:2"),
                            }
                        }
                    });
                }
            });

            var typeInfoResolver = new DefaultJsonTypeInfoResolver { };
            typeInfoResolver.Modifiers.Add(jsonTypeInfo =>
            {
                if (typeof(lazyzu.Jira.Database.Querier.Issue.Contract.ICustomFieldKey).IsAssignableFrom(jsonTypeInfo.Type))
                {
                    var propertiesToRemove = new List<JsonPropertyInfo>();

                    foreach (var property in jsonTypeInfo.Properties)
                    {
                        if (nameof(lazyzu.Jira.Database.Querier.Issue.Contract.ICustomFieldKey.ProjectionType).Equals(property.Name))
                        {
                            propertiesToRemove.Add(property);
                        }
                    }

                    foreach (var propertyToRemove in propertiesToRemove) jsonTypeInfo.Properties.Remove(propertyToRemove);
                }
            });

            Console.WriteLine(JsonSerializer.Serialize(new
            {
                Users = referenceUsers,
                Projects = referenceProjects,
                Issue = generatedIssue
            }, options: new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                TypeInfoResolver = typeInfoResolver
            }));

            builder.AddJiraDatabaseSchemaByInMemoryFake(inMemoryFakeContext);
            builder.AddDefaultJiraUserQuery();
            builder.AddDefaultJiraProjectQuery();

            builder.AddDefaultJiraIssueQuery(new ICustomFieldSource[]
            {
            new CustomFieldSource(new CustomFieldDefine("CascadingSelectExample", cascadingSelectFieldKey))
            });

            return inMemoryFakeContext;
        }

        public static void Configure_RealDatabase(WebApplicationBuilder builder
            , string jiraConnectionString = "..."
            , Uri jiraWebUri = null)
        {
            builder.AddJiraDatabaseSchema(jiraConnectionString, jiraWebUri);
            builder.AddDefaultJiraUserQuery();
            builder.AddDefaultJiraProjectQuery();
            builder.AddDefaultJiraIssueQuery(new ICustomFieldSource[]
            {
            new OOOCustomFieldSource()
            });
        }

        private class CustomFieldSource : ICustomFieldSource
        {
            private readonly CustomFieldDefine[] customFieldDefine;

            public CustomFieldSource(params CustomFieldDefine[] customFieldDefine)
            {
                this.customFieldDefine = customFieldDefine;
            }

            public IEnumerable<CustomFieldDefine> GetEnumerable()
            {
                return customFieldDefine;
            }
        }

        private class OOOCustomFieldSource : ICustomFieldSource
        {
            public IEnumerable<CustomFieldDefine> GetEnumerable()
            {
                yield break;
                // yield return new CustomFieldDefine(nameof(lazyzu.Jira.Database.OOO.Issue.CustomField.SameAsJiraFieldName), lazyzu.Jira.Database.OOO.Issue.CustomField.SameAsJiraFieldName);
            }
        }
    }
}
