using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project;
using lazyzu.Jira.Database.Querier.Project.Contract;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver
{
    public interface IProjectFieldKeyResolver
    {
        IEnumerable<Project.Contract.FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines);

        IEnumerable<FieldKey> Resolve(GraphQLField field
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines);
    }

    public class ProjectFieldKeyResolver : IProjectFieldKeyResolver
    {
        protected readonly IUserFieldKeyResolver userFieldKeyResolver;

        public ProjectFieldKeyResolver(IUserFieldKeyResolver userFieldKeyResolver)
        {
            this.userFieldKeyResolver = userFieldKeyResolver;
        }

        public IEnumerable<FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines)
        {
            foreach (var subField in subFields)
            {
                if (nameof(Project.IJiraProject.Lead).ToCamelCase().Equals(subField.Key))
                {
                    var subFiledKeyOfLead = userFieldKeyResolver.Resolve(subField.Value.Field, fragmentDefines).ToArray();
                    yield return ProjectFieldSelection.ProjectLeadWithField(subFiledKeyOfLead);
                }
                else if (FieldKeyMap.TryGetValue(subField.Key, out var fieldKey)) yield return fieldKey;
            }
        }

        public IEnumerable<FieldKey> Resolve(GraphQLField field
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines)
        {
            var fieldSelections = field?.SelectionSet?.Selections;
            return Resolve(fieldSelections, fragmentDefines);
        }

        private IEnumerable<FieldKey> Resolve(IEnumerable<ASTNode> selections
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines)
        {
            if (selections?.Any() ?? false)
            {
                foreach (var selection in selections)
                {
                    if (selection is GraphQLParser.AST.GraphQLField fieldSelection)
                    {
                        var fieldName = fieldSelection.Name.StringValue;
                        if (nameof(Project.IJiraProject.Lead).ToCamelCase().Equals(fieldName))
                        {
                            var subFiledKeyOfLead = userFieldKeyResolver.Resolve(fieldSelection, fragmentDefines).ToArray();
                            yield return ProjectFieldSelection.ProjectLeadWithField(subFiledKeyOfLead);
                        }
                        else if (FieldKeyMap.TryGetValue(fieldName, out var fieldKey)) yield return fieldKey;
                    }
                    else if (selection is GraphQLParser.AST.GraphQLFragmentSpread fragmentSelection)
                    {
                        var selectedDefine = fragmentDefines?.FirstOrDefault(fragment => fragment.FragmentName.Name.StringValue.Equals(fragmentSelection.FragmentName.Name.StringValue));

                        if (selectedDefine != null && ProjectGraphType.TypeName.Equals(selectedDefine.TypeCondition.Type.Name.StringValue))
                        {
                            var fragmentFieldSelections = selectedDefine?.SelectionSet?.Selections;
                            foreach (var fieldKey in Resolve(fragmentFieldSelections, fragmentDefines)) yield return fieldKey;
                        }
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
