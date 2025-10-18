using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueLinkGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueLink>
    {
        public IssueLinkGraphType()
        {
            Name = "IssueLink";

            Field(l => l.Id);
            Field(l => l.LinkType);
            Field(l => l.InwardIssueId);
            Field(l => l.OutwardIssueId);
        }
    }

    public class IssueLinkTypeGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueLinkType>
    {
        public IssueLinkTypeGraphType()
        {
            Name = "IssueLinkType";

            Field(t => t.Id);
            Field(t => t.Name);
            Field(t => t.Inward);
            Field(t => t.Outward);
        }
    }
}
