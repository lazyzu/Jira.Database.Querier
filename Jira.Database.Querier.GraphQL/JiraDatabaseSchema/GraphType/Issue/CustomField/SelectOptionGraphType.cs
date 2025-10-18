using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue.CustomField
{
    public class SelectOptionGraphType : ObjectGraphType<Querier.Issue.Fields.Custom.ISelectOption>
    {
        public SelectOptionGraphType()
        {
            Name = "SelectOption";

            Field(s => s.Id);
            Field(s => s.Value);
            Field(s => s.Disabled);
        }
    }

    public class SelectCollectionGraphType : ObjectGraphType<Querier.Issue.Fields.Custom.ISelectCollection>
    {
        public SelectCollectionGraphType()
        {
            Name = "SelectCollection";

            Field(s => s.Selections);
        }
    }

    public class CascadingSelectGraphType : ObjectGraphType<Querier.Issue.Fields.Custom.ICascadingSelection>
    {
        public CascadingSelectGraphType()
        {
            Name = "CascadingSelection";

            Field(s => s.CascadingSelections);
        }
    }
}
