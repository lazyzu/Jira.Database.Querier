using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project
{
    public class ProjectCategoryGraphType : ObjectGraphType<Querier.Project.Fields.IProjectCategory>
    {
        public ProjectCategoryGraphType()
        {
            Name = "ProjectCategory";

            Field(g => g.Id);
            Field(g => g.Name);
            Field(g => g.Description);
        }
    }
}
