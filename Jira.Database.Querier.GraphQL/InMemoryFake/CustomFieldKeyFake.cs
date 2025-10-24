using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;

namespace lazyzu.Jira.Database.Querier.GraphQL.InMemoryFake
{
    public static class CustomFieldKeyFake
    {
        private static int fieldIdIndex = 0;
        public static CustomFieldKey<TFieldScheme> BuildCustomFieldKey<TFieldScheme>(string fieldName)
        {
            return new CustomFieldKey<TFieldScheme>(fieldName, fieldIdIndex++);
        }

        public static UserCustomFieldKey<TFieldScheme> BuildUserCustomFieldKey<TFieldScheme>(string fieldName, User.Contract.FieldKey[] fields)
        {
            return new UserCustomFieldKey<TFieldScheme>(fieldName, fieldIdIndex++)
            {
                Fields = fields
            };
        }

        private static int fieldOptionIdIndex = 0;
        public static ISelectOption BuildOption(string name, bool disabled = false)
        {
            return new SelectOption
            {
                Id = fieldOptionIdIndex++,
                Value = name,
                Disabled = disabled
            };
        }
    }
}
