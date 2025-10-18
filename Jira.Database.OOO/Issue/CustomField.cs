using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;

namespace lazyzu.Jira.Database.OOO.Issue
{
    public partial class CustomField
    {
        // Please refer to ../docs/how-to-define-custom-field.md

        // com.atlassian.jira.plugin.system.customfieldtypes:float, value field in Jira DB: customfieldvalue.NUMBERVALUE
        public static readonly CustomFieldKey<NumberCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<NumberCustomFieldSchema>("Jira Field Name", 123);

        // com.atlassian.jira.plugin.system.customfieldtypes:datetime, value field in Jira DB: customfieldvalue.DATEVALUE
        // public static readonly CustomFieldKey<DateTimeCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<DateTimeCustomFieldSchema>("Jira Field Name", JiraFieldId);

        // com.atlassian.jira.plugin.system.customfieldtypes:textfield, value field in Jira DB: customfieldvalue.STRINGVALUE
        // public static readonly CustomFieldKey<StringCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<StringCustomFieldSchema>("Jira Field Name", JiraFieldId);

        // com.atlassian.jira.plugin.system.customfieldtypes:textarea, value field in Jira DB: customfieldvalue.TEXTVALUE
        // public static readonly CustomFieldKey<TextCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<TextCustomFieldSchema>("Jira Field Name", JiraFieldId);

        // com.atlassian.jira.plugin.system.customfieldtypes:labels, value field in Jira DB: label.LABEL
        // public static readonly CustomFieldKey<LabelCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<LabelCustomFieldSchema>("Jira Field Name", JiraFieldId);

        // com.atlassian.jira.plugin.system.customfieldtypes:userpicker, value field in Jira DB: customfieldvalue.STRINGVALUE
        // public static readonly CustomFieldKey<UserCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<UserCustomFieldSchema>("Jira Field Name", JiraFieldId);

        // com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker, value field in Jira DB: customfieldvalue.STRINGVALUE
        // public static readonly CustomFieldKey<MultiUserCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<MultiUserCustomFieldSchema>("Jira Field Name", JiraFieldId);

        // com.atlassian.jira.plugin.system.customfieldtypes:select, value field in Jira DB: customfieldvalue.STRINGVALUE
        // public static readonly CustomFieldKey<SelectCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<SelectCustomFieldSchema>("Jira Field Name", JiraFieldId);

        // com.atlassian.jira.plugin.system.customfieldtypes:multiselect, value field in Jira DB: customfieldvalue.STRINGVALUE
        // public static readonly CustomFieldKey<MultiSelectCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<MultiSelectCustomFieldSchema>("Jira Field Name", JiraFieldId);

        // com.atlassian.jira.plugin.system.customfieldtypes:cascadingselect, value field in Jira DB: customfieldvalue.STRINGVALUE
        // public static readonly CustomFieldKey<CascadingSelectCustomFieldSchema> SameAsJiraFieldName = new CustomFieldKey<CascadingSelectCustomFieldSchema>("Jira Field Name", JiraFieldId);
    }
}
