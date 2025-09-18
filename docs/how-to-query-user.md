# How to query User

## Specifying Query Fields
The user field options are located in the `UserFieldSelection` class within the `lazyzu.Jira.Database.Querier` namespace. The available fields include:
- `UserKey`
- `UserName`
- `UserDisplayName`
- `UserEmail`
- `UserActive`
- `UserAvatar`
- `UserGroup`
- `UserAppId`
- `UserCwdId`

You can specify the required fields using, for example, `lazyzu.Jira.Database.Querier.UserFieldSelection.UserKey`.

## Querying a Single User

Methods `User.GetUserByNameAsync` and `User.GetUserByKeyAsync` are provided for querying a single user. Here is an example:

```csharp
public async Task QueryUser(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                        , logger: null
                                                        , resetCache: true))
    {
        var targetUserName = "user-name";
        var result = await querier.User.GetUserByNameAsync(targetUserName, fields: new User.Contract.FieldKey[]
        {
            UserFieldSelection.UserName,
            UserFieldSelection.UserKey,
            UserFieldSelection.UserEmail
        } /*UserFieldSelection.All.ToArray()*/, cancellationToken);

        // If null fields argument => return default fields: user name & key
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

## Querying Multiple Users

Methods `User.GetUsersByNameAsync` and `User.GetUsersByKeyAsync` are provided for querying multiple users. Here is an example:

```csharp
public async Task QueryMultipleUsers(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                        , logger: null
                                                        , resetCache: true))
    {
        IEnumerable<string> targetUserNames = new string[]
        {
            //...,
        };
        var result = await querier.User.GetUsersByNameAsync(targetUserNames, fields: new User.Contract.FieldKey[]
        {
            UserFieldSelection.UserName,
            UserFieldSelection.UserKey,
            UserFieldSelection.UserEmail
        }, cancellationToken);

        // If null fields argument => return default fields: user name & key
    }
}
```

## Searching Users and Querying Information

The `User.SearchUserAsync` method allows searching users based on the `Name`, `DisplayName`, `Email`, and `Active` fields and returns the specified field details. Here is an example:

```csharp
public async Task SearchWithField(CancellationToken cancellationToken)
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext
                                                        , logger: null
                                                        , resetCache: true))
    {
        var result = await querier.User.SearchUserAsync(spec => new QuerySpecification.IQuerySpecification[]
        {
            spec.Name(name => EF.Functions.Like(name, "SomeNamePrefix%")),
            spec.Active(isActive => isActive > 0)
        }, fields: new User.Contract.FieldKey[]
        {
            UserFieldSelection.UserName,
            UserFieldSelection.UserKey,
            UserFieldSelection.UserEmail
        }, cancellationToken);

        // If null fields argument => return default fields: user name & key
    }
}
```