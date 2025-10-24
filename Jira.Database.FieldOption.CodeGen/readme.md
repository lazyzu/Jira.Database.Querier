# How to Automatically Generate Issue Field Options

The `Jira.Database.FieldOption.CodeGen` library supports generating field options by providing a Jira database connection string and definitions for Issue Custom Fields.
This allows you to easily package Jira variable options within your organization into a library.

Subsequently, whether you need to compare field values or have a basis for searching through field values.
(it’s possible to directly compare by id or name, but having a list of options should make development smoother and maintenance easier)

About `Jira.Database.FieldOption.CodeGen`, you can refer to the [source code](https://github.com/lazyzu/Jira.Database.Querier/tree/0.0.1/Jira.Database.FieldOption.CodeGen).
Or use [NuGet](https://www.nuget.org/packages/lazyzu.Jira.Database.FieldOption.CodeGen) for referencing.

Note that `Jira.Database.FieldOption.CodeGen` currently only supports MySQL databases for Jira.
Detailed examples can be found in the [Jira.Database.OOO](https://github.com/lazyzu/Jira.Database.Querier/tree/0.0.1/Jira.Database.OOO) project.

## Generated Items

### Native Fields
* IssueType
* IssuePriority
* IssueStatus
* IssueStatusCategory
* IssueSecurityLevel
* IssueResolution
* IssueLinkType

The generated field selection classes will be placed according to the settings in the `SourceGeneration.json` configuration file under `NativeFieldSelection.IssueFieldSelectionOutputNamespace`.
If set to `lazyzu.Jira.Database.OOO.Issue`, then the individual field selection classes correspond as follows:
Field | Selection class
--- | ---
IssueType | lazyzu.Jira.Database.OOO.Issue.IssueTypeSelection
IssuePriority | lazyzu.Jira.Database.OOO.Issue.IssuePrioritySelection
IssueStatus | lazyzu.Jira.Database.OOO.Issue.IssueStatusSelection
IssueStatusCategory | lazyzu.Jira.Database.OOO.Issue.IssueStatusCategorySelection
IssueSecurityLevel | lazyzu.Jira.Database.OOO.Issue.IssueSecurityLevelSelection
IssueResolution | lazyzu.Jira.Database.OOO.Issue.IssueResolutionSelection
IssueLinkType | lazyzu.Jira.Database.OOO.Issue.IssueLinkTypeSelection


### Select / multiselect / cascadingselect custom fields

Refer to the example definition below. The field option list classes will be automatically generated in the namespace `CustomFieldSelection` where you declare your fields.
Unlike Native Fields, this is not based on `SourceGeneration.json`
Note that the following is an example, and the declared items must match the existing field definitions in your organization’s Jira database.

```csharp
namespace lazyzu.Jira.Database.OOO.Issue;

public partial class CustomField
{
    // com.atlassian.jira.plugin.system.customfieldtypes:select
    public static readonly CustomFieldKey<SelectCustomFieldSchema> CountryCode
        = new CustomFieldKey<SelectCustomFieldSchema>(name: "CountryCode Field Name in Jira", id: countryCodeFieldIdInJira);

    // com.atlassian.jira.plugin.system.customfieldtypes:multiselect
    public static readonly CustomFieldKey<MultiSelectCustomFieldSchema> Fillings 
        = new CustomFieldKey<MultiSelectCustomFieldSchema>(name: "Fillings Field Name", id: fillingsFieldIdInJira);

    // com.atlassian.jira.plugin.system.customfieldtypes:cascadingselect
    public static readonly CustomFieldKey<CascadingSelectCustomFieldSchema> Region 
        = new CustomFieldKey<CascadingSelectCustomFieldSchema>(name: "Region Field Name in Jira", id: regionFieldIdInJira);
}
```

In the example, because the declaration is in the `lazyzu.Jira.Database.OOO.Issue namespace`, the generated option lists will be located in the `lazyzu.Jira.Database.OOO.Issue.CustomFieldSelection` namespace.
Field | Option class
--- | ---
CountryCode | lazyzu.Jira.Database.OOO.Issue.CustomFieldSelection.CountryCode
Fillings | lazyzu.Jira.Database.OOO.Issue.CustomFieldSelection.Fillings
Region | lazyzu.Jira.Database.OOO.Issue.CustomFieldSelection.Region

### User custom field

Since the return type of User custom fields is User, similar to Native Fields such as `Assignee` and `Reporter`, the default return is `user name` and `user key` ([based on JiraDatabaseQuerierBuilder configuration](https://github.com/lazyzu/Jira.Database.Querier/tree/0.0.1\docs\querier-builder-configuration.md)).

Each of them can specify the individual required detail fields through`lazyzu.Jira.Database.Querier.IssueFieldSelection.AssigneeWithField` or `lazyzu.Jira.Database.Querier.IssueFieldSelection.ReporterWithField`.

However, since the User custom field declaration does not have a `WithField`method, this part will be supplemented through automatic generation, directly produced in the same class as the field declaration (thus requiring the class to be declared as partial).

```csharp
namespace lazyzu.Jira.Database.OOO.Issue;

public partial class CustomField
{
    // com.atlassian.jira.plugin.system.customfieldtypes:userpicker
    public static readonly CustomFieldKey<UserCustomFieldSchema> Coordinator 
        = new CustomFieldKey<UserCustomFieldSchema>(name: "Coordinator Field Name in Jira", id: coordinatorFieldIdInJira);

    /* Generated Example
     * public static ICustomFieldKey Coordinator_WithField(lazyzu.Jira.Database.Querier.User.Contract.FieldKey[] userFields)
     * {
     *     return new UserCustomFieldKey<UserCustomFieldSchema>("Coordinator Field Name in Jira", coordinatorFieldIdInJira)
     *     {
     *         Fields = userFields
     *     };
     * }
     */

    // 
    public static readonly CustomFieldKey<MultiUserCustomFieldSchema> Reviewers 
        = new CustomFieldKey<MultiUserCustomFieldSchema>(name: "Reviewers Field Name in Jira", id: reviewersFieldIdInJira);
}
```


## SourceGeneration.json
Example configuration:
```json
{
  "Database": {
    "Type": "MySQL",
    "ConnectionString": "..."   // For reading and summarizing field options from Jira Database
  },
  "NativeFieldSelection": {
    "IssueFieldSelectionOutputNamespace": "lazyzu.Jira.Database.OOO.Issue"    // For native field option generation
  }
}
```

Specify this file as `AdditionalFile` for the analyzer to use:
```xml
<ItemGroup>
  <PackageReference Include="Jira.Database.FieldOption.CodeGen" Version="0.0.1" /> <!--For field selection generate (include type, status, priority... & custom fields)-->
  <AdditionalFiles Include="SourceGeneration.json" /> <!--Connection & Output config define for field selection generate-->
</ItemGroup>
```
