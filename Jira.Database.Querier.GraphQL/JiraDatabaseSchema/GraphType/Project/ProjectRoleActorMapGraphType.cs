using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project
{
    public class ProjectRoleActorMapGraphType : ObjectGraphType<Querier.Project.Fields.IProjectRoleActorMap>
    {
        public ProjectRoleActorMapGraphType()
        {
            Name = "ProjectRoleActorMap";

            Field(m => m.Id);
            Field(m => m.Name);
            Field(m => m.Description);
            Field(m => m.Actors);
        }
    }

    public class ProjectRoleActorGraphType : ObjectGraphType<Querier.Project.Fields.IProjectRoleActor>
    {
        public ProjectRoleActorGraphType()
        {
            Name = "ProjectRoleActor";

            Field(a => a.Type);
            Field(a => a.Value);
        }
    }
}
