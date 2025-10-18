using lazyzu.Jira.Database.Querier.User.Contract;
using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver
{
    public interface IUserFieldKeyResolver
    {
        IEnumerable<User.Contract.FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields);

        IEnumerable<FieldKey> Resolve(GraphQLField field);
    }

    public class UserFieldKeyResolvee : IUserFieldKeyResolver
    {
        public IEnumerable<FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields)
        {
            foreach (var subField in subFields)
            {
                if (FieldKeyMap.TryGetValue(subField.Key, out var fieldKey)) yield return fieldKey;
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
                        if (FieldKeyMap.TryGetValue(fieldName, out var fieldKey)) yield return fieldKey;
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
