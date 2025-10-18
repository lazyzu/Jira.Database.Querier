using GraphQL.Types;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Shared
{
    public class AvatarUrlGraphType : ObjectGraphType<Querier.Avatar.IAvatarUrl>
    {
        public AvatarUrlGraphType()
        {
            Name = "AvatarUrl";

            Field(a => a.XSmall);
            Field(a => a.Small);
            Field(a => a.Medium);
            Field(a => a.Large);
        }
    }
}
