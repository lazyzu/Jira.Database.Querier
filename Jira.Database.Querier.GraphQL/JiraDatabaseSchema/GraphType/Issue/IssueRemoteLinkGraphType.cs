using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueRemoteLinkGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueRemoteLink>
    {
        public IssueRemoteLinkGraphType()
        {
            Name = "IssueRemoteLink";

            Field(l => l.Id);
            Field(l => l.RemoteUrl);
            Field(l => l.Title);
            Field(l => l.Summary);
            Field(l => l.Relationship);
        }
    }
}
