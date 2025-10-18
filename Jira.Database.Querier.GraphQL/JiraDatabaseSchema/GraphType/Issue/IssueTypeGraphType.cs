using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueTypeGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueType>
    {
        public IssueTypeGraphType()
        {
            Name = "IssueType";

            Field(t => t.Id);
            Field(t => t.Name);
            Field(t => t.Description);
            Field(t => t.IsSubTask);
        }
    }

    public class IssueTypeSchemeGraphType : ObjectGraphType<Querier.Project.Fields.IIssueTypeScheme>
    {
        public IssueTypeSchemeGraphType()
        {
            Name = "IssueTypeScheme";

            Field(s => s.Id);
            Field(s => s.IssueTypes);
        }
    }
}
