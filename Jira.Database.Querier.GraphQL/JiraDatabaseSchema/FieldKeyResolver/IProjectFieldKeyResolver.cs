using lazyzu.Jira.Database.Querier.Project.Contract;
using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver
{
    public interface IProjectFieldKeyResolver
    {
        IEnumerable<Project.Contract.FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields);

        IEnumerable<FieldKey> Resolve(GraphQLField field);
    }

    public class ProjectFieldKeyResolver : IProjectFieldKeyResolver
    {
        protected readonly IUserFieldKeyResolver userFieldKeyResolver;

        public ProjectFieldKeyResolver(IUserFieldKeyResolver userFieldKeyResolver)
        {
            this.userFieldKeyResolver = userFieldKeyResolver;
        }

        public IEnumerable<FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields)
        {
            foreach (var subField in subFields)
            {
                if (nameof(Project.IJiraProject.Lead).ToCamelCase().Equals(subField.Key))
                {
                    var subFiledKeyOfLead = userFieldKeyResolver.Resolve(subField.Value.Field).ToArray();
                    yield return ProjectFieldSelection.ProjectLeadWithField(subFiledKeyOfLead);
                }
                else if (FieldKeyMap.TryGetValue(subField.Key, out var fieldKey)) yield return fieldKey;
            }
        }

        public IEnumerable<FieldKey> Resolve(GraphQLField field)
        {
            var fieldSelections = field?.SelectionSet?.Selections;

            if (fieldSelections?.Any() ?? false)
            {
                foreach (var fieldSelection in fieldSelections)
                {
                    if (fieldSelection is GraphQLParser.AST.GraphQLField _fieldSelection)
                    {
                        var fieldName = _fieldSelection.Name.StringValue;
                        if (nameof(Project.IJiraProject.Lead).ToCamelCase().Equals(fieldName))
                        {
                            var subFiledKeyOfLead = userFieldKeyResolver.Resolve(_fieldSelection).ToArray();
                            yield return ProjectFieldSelection.ProjectLeadWithField(subFiledKeyOfLead);
                        }
                        else if (FieldKeyMap.TryGetValue(fieldName, out var fieldKey)) yield return fieldKey;
                    }
                }
            }
        }

        private static readonly Dictionary<string, Project.Contract.FieldKey> FieldKeyMap
            = new Dictionary<string, Project.Contract.FieldKey>
            {
                { nameof(Project.IJiraProject.Id).ToCamelCase(), ProjectFieldSelection.ProjectId },
                { nameof(Project.IJiraProject.Name).ToCamelCase(), ProjectFieldSelection.ProjectName },
                { nameof(Project.IJiraProject.Url).ToCamelCase(), ProjectFieldSelection.ProjectUrl },
                { nameof(Project.IJiraProject.Lead).ToCamelCase(), ProjectFieldSelection.ProjectLeadWithField(UserFieldSelection.All.ToArray()) },
                { nameof(Project.IJiraProject.Description).ToCamelCase(), ProjectFieldSelection.ProjectDescription },
                { nameof(Project.IJiraProject.Key).ToCamelCase(), ProjectFieldSelection.ProjectKey },
                { nameof(Project.IJiraProject.Avatar).ToCamelCase(), ProjectFieldSelection.ProjectAvatar },
                { nameof(Project.IJiraProject.Type).ToCamelCase(), ProjectFieldSelection.ProjectType },
                { nameof(Project.IJiraProject.Category).ToCamelCase(), ProjectFieldSelection.ProjectCategory },
                { nameof(Project.IJiraProject.ProjectRoles).ToCamelCase(), ProjectFieldSelection.ProjectRole },
                { nameof(Project.IJiraProject.IssueTypeScheme).ToCamelCase(), ProjectFieldSelection.ProjectIssueType },
                { nameof(Project.IJiraProject.Components).ToCamelCase(), ProjectFieldSelection.ProjectComponent },
                { nameof(Project.IJiraProject.Versions).ToCamelCase(), ProjectFieldSelection.ProjectVersion },
                { nameof(Project.IJiraProject.SecurityLevels).ToCamelCase(), ProjectFieldSelection.ProjectIssueSecurityLevel },
            };
    }
}
