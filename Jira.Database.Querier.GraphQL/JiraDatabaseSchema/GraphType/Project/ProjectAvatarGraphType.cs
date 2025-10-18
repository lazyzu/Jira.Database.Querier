using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Project
{
    public class ProjectAvatarGraphType : ObjectGraphType<Querier.Project.Fields.IProjectAvatar>
    {
        public ProjectAvatarGraphType()
        {
            Name = "ProjectAvatar";

            Field(a => a.Id);
            Field(a => a.Urls);
        }
    }
}
