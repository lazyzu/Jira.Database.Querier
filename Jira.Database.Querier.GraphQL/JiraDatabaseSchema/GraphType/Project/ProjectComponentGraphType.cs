using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project
{
    public class ProjectComponentGraphType : ObjectGraphType<Querier.Project.Fields.IProjectComponent>
    {
        public ProjectComponentGraphType()
        {
            Name = "ProjectComponent";

            Field(c => c.Id);
            Field(c => c.Name);
            Field(c => c.Description);
            Field(c => c.Archived);
            Field(c => c.Deleted);
        }
    }

    public class FullProjectComponentGraphType : ObjectGraphType<Querier.Project.Fields.IFullProjectComponent>
    {
        public FullProjectComponentGraphType()
        {
            Name = "FullProjectComponent";

            Field(c => c.Id);
            Field(c => c.Name);
            Field(c => c.Description);
            Field(c => c.Archived);
            Field(c => c.Deleted);
        }
    }
}
