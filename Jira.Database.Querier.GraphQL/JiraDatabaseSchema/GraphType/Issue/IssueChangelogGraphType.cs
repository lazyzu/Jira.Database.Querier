using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueChangelogGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueChangelog>
    {
        public IssueChangelogGraphType()
        {
            Name = "IssueChangelog";

            Field(l => l.Id);
            Field(l => l.Author);
            Field(l => l.Created);
            Field(l => l.Items);
        }
    }

    public class IssueChangelogItemGraphType : ObjectGraphType<Querier.Issue.Fields.IIssueChangelogItem>
    {
        public IssueChangelogItemGraphType()
        {
            Name = "IssueChangelogItem";

            Field(i => i.Field);
            Field(i => i.FieldType);
            Field(i => i.OldValue);
            Field(i => i.OldString);
            Field(i => i.NewValue);
            Field(i => i.NewString);
        }
    }
}
