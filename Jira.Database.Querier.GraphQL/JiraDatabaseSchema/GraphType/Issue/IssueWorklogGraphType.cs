using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueWorklogGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueWorklog>
    {
        public IssueWorklogGraphType()
        {
            Name = "IssueWorklog";

            Field(l => l.Id);
            Field(l => l.Author);
            Field(l => l.Created);
            Field(l => l.UpdateAuthor);
            Field(l => l.Updated);
            Field(l => l.Started);
            Field(l => l.TimeSpent);
            Field(l => l.Comment);
        }
    }
}
