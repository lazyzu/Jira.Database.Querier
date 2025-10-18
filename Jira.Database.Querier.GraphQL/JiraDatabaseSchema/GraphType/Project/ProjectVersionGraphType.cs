using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project
{
    public class ProjectVersionGraphType : ObjectGraphType<Querier.Project.Fields.IProjectVersion>
    {
        public ProjectVersionGraphType()
        {
            Name = "ProjectVersion";

            Field(v => v.Id);
            Field(v => v.Name);
            Field(v => v.Description);
            Field(v => v.Archived);
            Field(v => v.StartDate);
            Field(v => v.ReleaseDate);
        }
    }

    public class FullProjectVersionGraphType : ObjectGraphType<Querier.Project.Fields.IFullProjectVersion>
    {
        public FullProjectVersionGraphType()
        {
            Name = "FullProjectVersion";

            Field(v => v.Id);
            Field(v => v.Name);
            Field(v => v.Description);
            Field(v => v.Archived);
            Field(v => v.StartDate);
            Field(v => v.ReleaseDate);
        }
    }
}
