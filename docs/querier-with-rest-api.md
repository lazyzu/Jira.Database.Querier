# Querier with REST API

The Jira database does not contain all information, such as Attachments and Avatars.
The [Jira.Database.Querier.RestApi](../Jira.Database.Querier.RestApi/) library provides an extension method set to connect with the Jira REST API to obtain these details.
Currently, it supports downloading Attachments, as shown in the example below.

## Example Usage
```csharp
using lazyzu.Jira.Database.Querier;

public async Task DownloadAttachment()
{
    using (var querier = JiraDatabaseQuerierBuilder.Build(BuildJiraContext, null))
    {
        var sampleIssue = await querier.Issue.GetIssueAsync("issue-key", fields: new Issue.Contract.FieldKey[]
        {
            IssueFieldSelection.Attachments
        });

        var authencator = Authenticator.FromBasic("username", "password");
        foreach (var attachment in sampleIssue.Attachments)
        {
            using (var fileStream = await attachment.Download(authencator))
            {
                // Process the downloaded file stream here
            }
        }
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
