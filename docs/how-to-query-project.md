# How to query Project

## Specifying Query Fields
The project field options are located in the `ProjectFieldSelection` class within the `lazyzu.Jira.Database.Querier` namespace. The available fields include:
- `ProjectName`
- `ProjectUrl`
- `ProjectLead`
- `ProjectDescription`
- `ProjectKey`
- `ProjectAvatar`
- `ProjectType`
- `ProjectCategory`
- `ProjectRole`
- `ProjectIssueType`
- `ProjectComponent`
- `ProjectVersion`
- `ProjectIssueSecurityLevel`

You can specify the required fields using, for example, `lazyzu.Jira.Database.Querier.ProjectFieldSelection.ProjectName`.

### ProjectLead Field
The `ProjectLead` field is of type `User`. By default, only the `user name` and `user key` fields are returned. However, you can specify additional fields for `ProjectLead` using `lazyzu.Jira.Database.Querier.ProjectFieldSelection.ProjectLeadWithField`. For example:
```csharp
lazyzu.Jira.Database.Querier.ProjectFieldSelection.ProjectLeadWithField(new User.Contract.FieldKey[] 
{
    UserFieldSelection.UserName,
    UserFieldSelection.UserEmail
})
```

## Querying a Single Project

The `Project.GetProjectAsync` method allows querying a single project by its `project id` or `project key`. Here is an example:

```csharp
public async Task QueryProject(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                        , logger: null
                                                        , resetCache: true))
    {
        var targetProjectKey = "project-key";
        var result = await querier.Project.GetProjectAsync(targetProjectKey /*or by id*/, fields: new Project.Contract.FieldKey[]
        {
            ProjectFieldSelection.ProjectKey,
            ProjectFieldSelection.ProjectName,
            ProjectFieldSelection.ProjectLead
        } /*ProjectFieldSelection.All.ToArray()*/, cancellationToken);

        // If null fields argument => use default: project key & name
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

## Querying Multiple Projects

The `Project.GetProjectsAsync` method allows querying multiple projects by their `project id` or `project key`. Here is an example:

```csharp
public async Task QueryProjects(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                        , logger: null
                                                        , resetCache: true))
    {
        IEnumerable<string> targetProjectKey = new string[]
        {
            //...,
        };
        var result = await querier.Project.GetProjectsAsync(targetProjectKey /*or by ids*/, fields: new Project.Contract.FieldKey[]
        {
            ProjectFieldSelection.ProjectKey,
            ProjectFieldSelection.ProjectName,
            ProjectFieldSelection.ProjectLead
        }, cancellationToken);

        // If null fields argument => use default: project key & name
    }
}
```

## Searching Projects and Querying Information

The `Project.SearchProjectAsync` method allows searching projects based on the `Id`, `Name`, `Url`, `Lead`, `Description`, `Key` and  `Type`  fields and returns the specified field details. Here is an example:

```csharp
public async Task SearchWithField(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                        , logger: null
                                                        , resetCache: true))
    {
        var result = await querier.Project.SearchProjectAsync(spec => new QuerySpecification.IQuerySpecification[]
        {
            spec.Name(name => EF.Functions.Like(name, "Test%"))
        }, fields: new Project.Contract.FieldKey[]
        {
            ProjectFieldSelection.ProjectKey,
            ProjectFieldSelection.ProjectName,
            ProjectFieldSelection.ProjectLead
        }, cancellationToken);

        // If null fields argument => use default: project key & name
    }
}
```

## Notes
If the `fields` argument is null, the default fields queried are `project key` and `project name`.
The `resetCache` parameter can be used to clear the cache before performing the query.
The `logger` parameter can be used to log query operations.