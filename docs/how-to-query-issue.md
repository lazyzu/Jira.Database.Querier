# How to query Issue

## Specifying Query Fields

The issue field options are located in the `IssueFieldSelection` class within the `lazyzu.Jira.Database.Querier` namespace. The available fields include:

- `IssueNum`
- `Project`
- `Key`
- `Summary`
- `Description`
- `CreateDate`
- `UpdateDate`
- `DueDate`
- `ResolutionDate`
- `SecurityLevel`
- `Assignee`
- `Reporter`
- `Environment`
- `Votes`
- `IssueStatus`
- `Priority`
- `Resolution`
- `IssueType`
- `Components`
- `AffectsVersions`
- `FixVersions`
- `Labels`
- `Comments`
- `Worklogs`
- `Changelogs`
- `ParentIssueId`
- `SubTaskIds`
- `IssueLinks`
- `RemoteLinks`
- `Attachments`

You can specify the required fields using, for example, `lazyzu.Jira.Database.Querier.IssueFieldSelection.Key`.

### Project Field

The `Project` field is of type `Project`. By default, only the `project key` and `project name` fields are returned. However, you can specify additional fields for `Project` using `lazyzu.Jira.Database.Querier.IssueFieldSelection.ProjectWithField`. For example:

```csharp
lazyzu.Jira.Database.Querier.IssueFieldSelection.ProjectWithField(new User.Contract.FieldKey[] 
{
    ProjectFieldSelection.ProjectKey,
    ProjectFieldSelection.ProjectName,
    ProjectFieldSelection.ProjectLead
})
```

### Assignee & Reporter Field

The `Assignee` & `Reporter` field is of type `User`. By default, only the `user name` and `user key` fields are returned. However, you can specify additional fields using `lazyzu.Jira.Database.Querier.IssueFieldSelection.AssigneeWithField` or `lazyzu.Jira.Database.Querier.IssueFieldSelection.ReporterWithField`. For example:

```csharp
lazyzu.Jira.Database.Querier.IssueFieldSelection.AssigneeWithField(new User.Contract.FieldKey[] 
{
    UserFieldSelection.UserName,
    UserFieldSelection.UserEmail
})
```
### Custom Field
For defining and querying custom fields, please refer to [Custom Field Define](./how-to-define-custom-field.md)

## Querying a Single Issue

The `Issue.GetIssueAsync` method allows querying a single issue by its `issue id` or `issue key`. Here is an example:

```csharp
public async Task QueryIssue(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                        , logger: null
                                                        , resetCache: true))
    {
        var result = await querier.Issue.GetIssueAsync("Issue-Key" /*or IssueId*/, fields:  new Issue.Contract.FieldKey[]
        {
            IssueFieldSelection.Summary,
            IssueFieldSelection.Project,
            IssueFieldSelection.Assignee,
        }, cancellationToken);

        // If null fields argument => use default: issue key & summary
    }
}

public JiraDatabaseQuerierBuilder JiraDatabaseQuerierBuilder = new Querier.JiraDatabaseQuerierBuilder(Constant.JiraApiHostUrl);

public EntityFrameworkCore.JiraContext BuildJiraContext()
    => new EntityFrameworkCore.MySQL.JiraContext(Constant.JiraReadonlyConnectionString);

public static class Constant
{
    public static readonly Uri JiraApiHostUrl = new Uri("...");
    public const string JiraReadonlyConnectionString = "...";
}
```

## Querying Multiple Issues

The `Issue.GetIssuesAsync` method allows querying multiple issues by their `issue id` or `issue key`. Here is an example:

```csharp
public async Task QueryIssues(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                              , logger: null
                                                              , resetCache: true))
    {
        IEnumerable<decimal> targetIssueIds = new decimal[]
        {
            //...,
        };

        var result = await querier.Issue.GetIssuesAsync(targetIssueIds /*or Issue keys*/, fields: new Issue.Contract.FieldKey[]
        {
            IssueFieldSelection.Summary,
            IssueFieldSelection.Project,
            IssueFieldSelection.Assignee,
        }, cancellationToken);

        // If null fields argument => use default: issue key & summary
    }
}
```

## Searching Issues and Querying Information

The `Issue.SearchIssueAsync` method allows searching issues based on the `IssueId`, `Project`, `Summary`, `Description`, `CreateDate`, `UpdateDate`, `DueDate`, `ResolutionDate`, `SecurityLevel`, `Assignee`, `Reporter`, `Environment`, `Votes`, `Status`, `StatusCategory`, `Priority`, `Resolution`, `Type`, `AffectsVersion`, `FixVersion`, and custom fields, and returns the specified field details. Here is an example:

```csharp
public async Task SearchWithField(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                              , logger: null
                                                              , resetCache: true))
    {
        var targetProjectKey = "project-key";
        var targetProject = await querier.Project.GetProjectAsync(targetProjectKey);

        // select ID from jiraissue where PROJECT = projectId and issuetype = 'issueTypeId' and CREATED > '5 months ago'
        // select ISSUE from customfieldvalue where CUSTOMFIELD = numberCustomFieldId and NUMBERVALUE > 10
        var issues = await querier.Issue.SearchIssueAsync(spec => new QuerySpecification.IQuerySpecification[]
        {
            spec.IssueProject(targetProject),
            spec.IssueType(IssueTypeSelection.Task),
            spec.IssueCreateDate(createDate => createDate > DateTime.Now.AddMonths(-5)),
            spec.CustomField(NumberCustomField, fieldValue => fieldValue > 10)
        }, fields: new Issue.Contract.FieldKey[]
        {
            IssueFieldSelection.Key,
            IssueFieldSelection.Summary,
            IssueFieldSelection.Project,
            IssueFieldSelection.Assignee,
        }, cancellationToken);

        // If null fields argument => use default: issue key & summary
    }
}
```

If you only need to search for issue IDs matching a specific searching condition, you can use the `Issue.SearchIssueIdAsync` method.
