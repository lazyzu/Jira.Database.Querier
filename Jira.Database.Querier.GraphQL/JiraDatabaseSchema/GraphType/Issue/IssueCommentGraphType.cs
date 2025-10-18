using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueCommentGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueComment>
    {
        public IssueCommentGraphType()
        {
            Name = "IssueComment";

            Field(c => c.Id);
            Field(c => c.Author);
            Field(c => c.Created);
            Field(c => c.UpdateAuthor);
            Field(c => c.Updated);
            Field(c => c.Body);
        }
    }
}
