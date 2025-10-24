using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.User;
using lazyzu.Jira.Database.Querier.User.Contract;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver
{
    public interface IUserFieldKeyResolver
    {
        IEnumerable<User.Contract.FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines);

        IEnumerable<FieldKey> Resolve(GraphQLField field
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines);
    }

    public class UserFieldKeyResolvee : IUserFieldKeyResolver
    {
        public IEnumerable<FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines)
        {
            foreach (var subField in subFields)
            {
                if (FieldKeyMap.TryGetValue(subField.Key, out var fieldKey)) yield return fieldKey;
            }
        }

        public IEnumerable<FieldKey> Resolve(GraphQLField field
            , GraphQLParser.AST.GraphQLFragmentDefinition[] fragmentDefines)
        {
            var fieldSelections = field?.SelectionSet?.Selections;
            return Resolve(field?.SelectionSet?.Selections, fragmentDefines);
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
                        if (FieldKeyMap.TryGetValue(fieldName, out var fieldKey)) yield return fieldKey;
                    }
                    else if (selection is GraphQLParser.AST.GraphQLFragmentSpread fragmentSelection)
                    {
                        var selectedDefine = fragmentDefines?.FirstOrDefault(fragment => fragment.FragmentName.Name.StringValue.Equals(fragmentSelection.FragmentName.Name.StringValue));

                        if (selectedDefine != null && UserGraphType.TypeName.Equals(selectedDefine.TypeCondition.Type.Name.StringValue))
                        {
                            var fragmentFieldSelections = selectedDefine?.SelectionSet?.Selections;
                            foreach (var fieldKey in Resolve(fragmentFieldSelections, fragmentDefines)) yield return fieldKey;
                        }
                    }
                }
            }
        }

        private static readonly Dictionary<string, User.Contract.FieldKey> FieldKeyMap
            = new Dictionary<string, User.Contract.FieldKey>
            {
                { nameof(Querier.User.IJiraUser.AppId).ToCamelCase(), UserFieldSelection.UserAppId },
                { nameof(Querier.User.IJiraUser.CwdId).ToCamelCase(), UserFieldSelection.UserCwdId },
                { nameof(Querier.User.IJiraUser.Key).ToCamelCase(), UserFieldSelection.UserKey },
                { nameof(Querier.User.IJiraUser.Username).ToCamelCase(), UserFieldSelection.UserName },
                { nameof(Querier.User.IJiraUser.DisplayName).ToCamelCase(), UserFieldSelection.UserDisplayName },
                { nameof(Querier.User.IJiraUser.Email).ToCamelCase(), UserFieldSelection.UserEmail },
                { nameof(Querier.User.IJiraUser.IsActive).ToCamelCase(), UserFieldSelection.UserActive },
                { nameof(Querier.User.IJiraUser.Avatar).ToCamelCase(), UserFieldSelection.UserAvatar },
                { nameof(Querier.User.IJiraUser.Groups).ToCamelCase(), UserFieldSelection.UserGroup },
            };
    }
}
