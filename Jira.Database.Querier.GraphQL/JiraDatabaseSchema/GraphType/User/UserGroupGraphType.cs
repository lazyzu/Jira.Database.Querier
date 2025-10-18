using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.User
{
    public class UserGroupGraphType : ObjectGraphType<Querier.User.Fields.IUserGroup>
    {
        public UserGroupGraphType()
        {
            Name = "UserGroup";

            Field(g => g.Id);
            Field(g => g.Name);
        }
    }
}
