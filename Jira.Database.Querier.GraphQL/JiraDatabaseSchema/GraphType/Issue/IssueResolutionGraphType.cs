using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueResolutionGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueResolution>
    {
        public IssueResolutionGraphType()
        {
            Name = "IssueResolution";

            Field(r => r.Id);
            Field(r => r.Name);
            Field(r => r.Description);
        }
    }
}
