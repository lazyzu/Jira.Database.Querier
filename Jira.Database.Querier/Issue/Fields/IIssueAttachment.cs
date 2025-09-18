using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public interface IIssueAttachment
    {
        decimal Id { get; }
        string FileName { get; }
        string Author { get; }
        DateTime Created { get; }
        decimal Size { get; }
        string MimeType { get; }
        Uri Content { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueAttachment : IIssueAttachment
    {
        [DefaultEquality]
        public decimal Id { get; init; }
        public string FileName { get; init; }
        public string Author { get; init; }
        public DateTime Created { get; init; }
        public decimal Size { get; init; }
        public string MimeType { get; init; }
        public Uri Content { get; init; }

        public override string ToString()
        {
            return $"{Id}: {FileName} (Author: {Author})";
        }
    }

    public interface IIssueAttachmentUrlBuilder
    {
        Uri BuildFrom(decimal? issueId, decimal attachmentId, string fileName);
    }

    public class IssueAttachmentUrlBuilder : IIssueAttachmentUrlBuilder
    {
        protected readonly Uri baseUrl;

        public IssueAttachmentUrlBuilder(Uri baseUrl) 
        {
            this.baseUrl = baseUrl;
        }

        public Uri BuildFrom(decimal? issueId, decimal attachmentId, string fileName)
        {
            if (Uri.TryCreate(baseUrl, $@"secure/attachment/{attachmentId}/{fileName}", out var attachmentUri)) return attachmentUri;
            else throw new Exception($"Not able create issue attachment url from {baseUrl}, {attachmentId} ({fileName})");
        }
    }

    public class IssueAttachmentProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly IIssueAttachmentUrlBuilder issueAttachmentUrlBuilder;
        protected readonly ILogger logger;

        public IssueAttachmentProjection(JiraContext jiraContext, IIssueAttachmentUrlBuilder issueAttachmentUrlBuilder, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.issueAttachmentUrlBuilder = issueAttachmentUrlBuilder;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Attachments
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var attachmentMap = await LoadAttachmentMap(issueIds, cancellationToken).ConfigureAwait(false);

                if (attachmentMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (attachmentMap.TryGetValue(issue.Id, out var attachments)) issue.Attachments = attachments;
                        else issue.Attachments = new IIssueAttachment[0];
                    }
                }
                else foreach (var issue in _issues)
                {
                    issue.Attachments = new IIssueAttachment[0];
                }
            }
        }

        protected virtual async Task<Dictionary<decimal?, IssueAttachment[]>> LoadAttachmentMap(decimal?[] issueIds, CancellationToken cancellationToken)
        {
            var query = jiraContext.fileattachment.AsNoTracking()
                .Where(fileattachment => issueIds.Contains(fileattachment.issueid))
                .Select(fileattachment => new
                {
                    fileattachment.ID,
                    fileattachment.issueid,
                    fileattachment.MIMETYPE,
                    fileattachment.FILENAME,
                    fileattachment.CREATED,
                    fileattachment.FILESIZE,
                    fileattachment.AUTHOR
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.GroupBy(dbModel => dbModel.issueid)
                .ToDictionary(issueIdGroup => issueIdGroup.Key
                            , issueIdGroup => issueIdGroup.Select(dbModel => new IssueAttachment
                            {
                                Id = dbModel.ID,
                                FileName = dbModel.FILENAME,
                                Author = dbModel.AUTHOR,
                                Created = dbModel.CREATED.Value,
                                Size = dbModel.FILESIZE.Value,
                                MimeType = dbModel.MIMETYPE,
                                Content = issueAttachmentUrlBuilder.BuildFrom(dbModel.issueid, dbModel.ID, dbModel.FILENAME)
                            }).ToArray());
        }
    }
}
