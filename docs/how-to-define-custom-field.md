# How to define Custom Field

Jira Issues allow for the customization of various fields, meaning that when querying data, you won’t be limited to built-in fields alone.
Instead, you can define custom fields according to your organization’s requirements to enhance the flexibility and utility of your queries.

## Supported Custom Field Types
Type | Jira Field Type | Value in Jira DB
--- | --- | ---
`CustomFieldKey<NumberCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:float` | `customfieldvalue.NUMBERVALUE`
`CustomFieldKey<DateTimeCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:datetime` | `customfieldvalue.DATEVALUE`
`CustomFieldKey<StringCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:textfield` | `customfieldvalue.STRINGVALUE`
`CustomFieldKey<TextCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:textarea` | `customfieldvalue.TEXTVALUE`
`CustomFieldKey<LabelCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:labels` | `label.LABEL`
`CustomFieldKey<UserCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:userpicker` | `customfieldvalue.STRINGVALUE` (user key)
`CustomFieldKey<MultiUserCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:multiuserpicker` | `customfieldvalue.STRINGVALUE` (user key)
`CustomFieldKey<SelectCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:select` | `customfieldvalue.STRINGVALUE` (option id)
`CustomFieldKey<MultiSelectCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:multiselect` | `customfieldvalue.STRINGVALUE` (option id)
`CustomFieldKey<CascadingSelectCustomFieldSchema>` | `com.atlassian.jira.plugin.system.customfieldtypes:cascadingselect` | `customfieldvalue.STRINGVALUE` (option id)

## Declaration and Querying Example
```csharp
// Declare custom field 
public static readonly CustomFieldKey<NumberCustomFieldSchema> NumberCustomField = new CustomFieldKey<NumberCustomFieldSchema>(name: "Real Field Name in Jira", id: RealFieldId);

public async Task QueryIssue_WithCustomField(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                        , logger: null
                                                        , resetCache: true))
    {
        var result = await querier.Issue.GetIssueAsync("Issue-Key" /*or input IssueId*/, fields: new Issue.Contract.FieldKey[]
        {
            IssueFieldSelection.Summary,
            IssueFieldSelection.Project,
            IssueFieldSelection.Assignee,
            NumberCustomField,  // Query with custom field
        }, cancellationToken);

        /* read value directly
         * var numberFieldValue = result.CustomFields.GetValue(NumberCustomField, defaultValue: null);
         */
        
        /* Try get value
         * if (result.CustomFields.TryGetValue(NumberCustomField, out var numberFieldValue))
         * {
         *     numberFieldValue.Value;
         * }
         */

         // output type changes based on generic definition during declaration
    }
}
```

Additionally, for selection-type fields such as:
* `CustomFieldKey<SelectCustomFieldSchema>`
* `CustomFieldKey<MultiSelectCustomFieldSchema>`
* `CustomFieldKey<CascadingSelectCustomFieldSchema>`
After retrieving these fields, you may need to perform comparisons. Referencing the guide on [How to Automatically Generate Issue Field Options](./how-to-auto-generate-field-option.md) can provide you with a list of options, making subsequent logic smoother and reducing the risk of scattered magic IDs throughout your code.
