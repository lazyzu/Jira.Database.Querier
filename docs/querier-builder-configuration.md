# Injecting Custom Services or Default Settings

The `JiraDatabaseQuerierBuilder` can define almost all internal services, including the following:

### Default Query Fields
- `UseDefaultIssueQueryField`
- `UseDefaultProjectQueryField`
- `UseDefaultUserQueryField`

### Field Projection Services
- `UseIssueSimpleFieldProjection`
- `UseIssueCustomFieldProjection`
- `UseProjectProjection`
- `UseUserProjection`

### Search Handlers
- `UseIssueQuerySpecificationHandler`
- `UseProjectQuerySpecificationHandler`
- `UseUserQuerySpecificationHandler`

### Issue-related Services
- `UseIssueKeyService`
- `UseIssuePriorityService`
- `UseIssueResolutionService`
- `UseIssueSecurityLevelService`
- `UseIssueStatusService`
- `UseIssueStatusCategoryService`
- `UseIssueTypeService`
- `UseIssueLinkService`
- `UseIssueCustomFieldService`

### Project-related Services
- `UseProjectKeyService`
- `UseProjectCategoryService`
- `UseProjectRoleService`

### URL Builders
- `UseIssueAttachmentUrlBuilder`
- `UseProjectAvatarUrlBuilder`
- `UseUserAvatarUrlBuilder`

### Example: Customizing Default Issue Query Fields and IssueKeyService

Below is an example of how to customize the default issue query fields and implement a custom `IssueKeyService`.

```csharp
public JiraDatabaseQuerierBuilder BuildQuerierBuilder()
{
    var builder = new Querier.JiraDatabaseQuerierBuilder(Constant.JiraApiHostUrl);

    builder.UseDefaultIssueQueryField(() => new Issue.Contract.FieldKey[]
    {
        IssueFieldSelection.Key,
        IssueFieldSelection.Summary,
        IssueFieldSelection.Labels,
    });

    builder.UseIssueKeyService(buildContext => new CustomIssueKeyService(buildContext.JiraContext));

    return builder;
}

public static class Constant
{
    public static readonly Uri JiraApiHostUrl = new Uri("...");
    public const string JiraReadonlyConnectionString = "...";
}

public class CustomIssueKeyService : IIssueKeyService
{
    private readonly JiraContext jiraContext;

    public CustomIssueKeyService(JiraContext jiraContext)
    {
        this.jiraContext = jiraContext;
    }

    public Task<Dictionary<string, decimal>> GetIssueIdAsync(IEnumerable<string> issueKeys, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<decimal?> GetIssueIdAsync(string issueKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool?> IsMovedAsync(string issueKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
```