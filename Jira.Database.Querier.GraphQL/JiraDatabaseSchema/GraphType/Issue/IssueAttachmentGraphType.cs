using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueAttachmentGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueAttachment>
    {
        public IssueAttachmentGraphType()
        {
            Name = "IssueAttachment";

            Field(a => a.Id);
            Field(a => a.FileName);
            Field(a => a.Author);
            Field(a => a.Created);
            Field(a => a.Size);
            Field(a => a.MimeType);
            Field(a => a.Content);
        }
    }
}
