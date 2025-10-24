using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.User
{
    public class UserGraphType : ObjectGraphType<Querier.User.IJiraUser>
    {
        public const string TypeName = "User";

        public UserGraphType()
        {
            Name = TypeName;

            Field(u => u.AppId).Description("Id in AppUser Table");
            Field(u => u.CwdId).Description("Id in CwdUser Table");
            Field(u => u.Key).Description("Key mapping between AppUser & CwdUser");
            Field(u => u.Username).Description("User name of Jira User");
            Field(u => u.DisplayName).Description("Display name of Jira User");
            Field(u => u.Email).Description("Email of Jira User");
            Field(u => u.IsActive).Description("Isactive of Jira User");
            Field(u => u.Avatar).Description("Avatar of Jira User");
            Field(u => u.Groups).Description("Groups of Jira User");
        }
    }
}
