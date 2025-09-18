using lazyzu.Jira.Database.Querier.RestApi;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier
{
    public static class AttachmentExtension
    {
        public static async Task<Stream> Download(this lazyzu.Jira.Database.Querier.Issue.Fields.IIssueAttachment attachment
            , IAuthenticator authenticator
            , CancellationToken cancellationToken = default)
        {
            var client = new HttpClient();
            return await Download(attachment, client, authenticator, cancellationToken);
        }

        public static async Task<Stream> Download(this lazyzu.Jira.Database.Querier.Issue.Fields.IIssueAttachment attachment
            , IHttpClientFactory clientFactory
            , IAuthenticator authenticator
            , CancellationToken cancellationToken = default)
        {
            var client = clientFactory.CreateClient();
            return await Download(attachment, client, authenticator, cancellationToken);
        }

        public static async Task<Stream> Download(this lazyzu.Jira.Database.Querier.Issue.Fields.IIssueAttachment attachment
            , HttpClient client
            , IAuthenticator authenticator
            , CancellationToken cancellationToken = default)
        {
            var requestRelativePath = attachment.Content;
            if (client.BaseAddress != null) requestRelativePath = attachment.Content.MakeRelativeUri(client.BaseAddress);

            var request = new HttpRequestMessage(HttpMethod.Get, requestRelativePath);
            authenticator.Authenticate(client, request);

            var response = await client.SendAsync(request, cancellationToken);
#if NET5_0_OR_GREATER
            return await response.Content.ReadAsStreamAsync(cancellationToken);
#else
            return await response.Content.ReadAsStreamAsync();
#endif
        }
    }
}
