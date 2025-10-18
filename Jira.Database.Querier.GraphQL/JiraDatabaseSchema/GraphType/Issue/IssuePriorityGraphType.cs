using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssuePriorityGraphType : ObjectGraphType<Querier.Issue.Fields.IIssuePriority>
    {
        public IssuePriorityGraphType()
        {
            Name = "IssuePriority";

            Field(p => p.Id);
            Field(p => p.Name);
            Field(p => p.Description);
        }
    }
}
