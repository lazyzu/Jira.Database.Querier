using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueStatusGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueStatus>
    {
        public IssueStatusGraphType()
        {
            Name = "IssueStatus";

            Field(s => s.Id);
            Field(s => s.Name);
            Field(s => s.Description);
            Field(s => s.Category);
        }
    }

    public class IssueStatusCategoryGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueStatusCategory>
    {
        public IssueStatusCategoryGraphType()
        {
            Name = "IssueStatusCategory";

            Field(c => c.Id);
            Field(c => c.Name);
        }
    }
}
