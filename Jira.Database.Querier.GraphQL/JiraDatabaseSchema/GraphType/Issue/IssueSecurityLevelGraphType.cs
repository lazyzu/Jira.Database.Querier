using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueSecurityLevelGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueSecurityLevel>
    {
        public IssueSecurityLevelGraphType()
        {
            Name = "IssueSecurityLevel";

            Field(l => l.Id);
            Field(l => l.Name);
            Field(l => l.Description);
            Field(l => l.Scheme);
        }
    }

    public class IssueSecurityLevelSchemeGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueSecurityLevelScheme>
    {
        public IssueSecurityLevelSchemeGraphType()
        {
            Name = "IssueSecurityLevelScheme";

            Field(s => s.Id);
            Field(s => s.Name);
            Field(s => s.Description);
        }
    }
}
