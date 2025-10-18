using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.User
{
    public class UserAvatarGraphType : ObjectGraphType<Querier.User.Fields.IUserAvatar>
    {
        public UserAvatarGraphType()
        {
            Name = "UserAvatar";

            Field(a => a.Id);
            Field(a => a.Urls);
        }
    }
}
