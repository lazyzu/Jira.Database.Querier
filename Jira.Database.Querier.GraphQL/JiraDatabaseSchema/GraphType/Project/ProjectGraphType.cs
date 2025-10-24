using GraphQL.Types;
using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project
{
    public class ProjectGraphType : ObjectGraphType<Querier.Project.IJiraProject>
    {
        public const string TypeName = "Project";

        public ProjectGraphType()
        {
            Name = TypeName;

            Field(p => p.Id).Description("Project Id");
            Field(p => p.Name).Description("Project Name");
            Field(p => p.Url).Description("Project Url");
            Field(p => p.Lead).Description("Project Lead");
            Field(p => p.Description).Description("Project Description");
            Field(p => p.Key).Description("Project Key");
            Field(p => p.Avatar).Description("Project Avatar");
            Field(p => p.Type).Description("Project Type");
            Field(p => p.Category).Description("Project Category");
            Field(p => p.ProjectRoles).Description("Project Roles");
            Field(p => p.IssueTypeScheme).Description("Project IssueTypeScheme");
            Field(p => p.Components).Description("Project Components");
            Field(p => p.Versions).Description("Project Versions");
            Field(p => p.SecurityLevels).Description("Project SecurityLevels");
        }

        public static void ProjectJsonSerializeOptimize(JsonTypeInfo jsonTypeInfo)
        {
            if (typeof(Querier.Project.Fields.IFullProjectComponent).IsAssignableFrom(jsonTypeInfo.Type))
            {
                var propertiesToRemove = new List<JsonPropertyInfo>();

                foreach (var property in jsonTypeInfo.Properties)
                {
                    if (nameof(Querier.Project.Fields.IFullProjectComponent.Project).Equals(property.Name))
                    {
                        propertiesToRemove.Add(property);
                    }
                }

                foreach (var propertyToRemove in propertiesToRemove) jsonTypeInfo.Properties.Remove(propertyToRemove);
            }
            else if (typeof(Querier.Project.Fields.IFullProjectVersion).IsAssignableFrom(jsonTypeInfo.Type))
            {
                var propertiesToRemove = new List<JsonPropertyInfo>();

                foreach (var property in jsonTypeInfo.Properties)
                {
                    if (nameof(Querier.Project.Fields.IFullProjectVersion.Project).Equals(property.Name))
                    {
                        propertiesToRemove.Add(property);
                    }
                }

                foreach (var propertyToRemove in propertiesToRemove) jsonTypeInfo.Properties.Remove(propertyToRemove);
            }
            else if (typeof(Querier.Issue.Fields.IIssueSecurityLevelScheme).IsAssignableFrom(jsonTypeInfo.Type))
            {
                var propertiesToRemove = new List<JsonPropertyInfo>();

                foreach (var property in jsonTypeInfo.Properties)
                {
                    if (nameof(Querier.Issue.Fields.IIssueSecurityLevelScheme.DefaultValue).Equals(property.Name))
                    {
                        propertiesToRemove.Add(property);
                    }
                }

                foreach (var propertyToRemove in propertiesToRemove) jsonTypeInfo.Properties.Remove(propertyToRemove);
            }
        }
    }
}
