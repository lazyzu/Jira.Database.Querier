using lazyzu.Jira.Database.Querier.User.Contract;
using System.Collections.Immutable;

namespace lazyzu.Jira.Database.Querier
{
    public static class UserFieldSelection
    {
        public static readonly FieldKey UserAppId = new FieldKey("UserAppId");
        public static readonly FieldKey UserCwdId = new FieldKey("UserCwdId");
        public static readonly FieldKey UserKey = new FieldKey("UserKey");
        public static readonly FieldKey UserName = new FieldKey("UserName");
        public static readonly FieldKey UserDisplayName = new FieldKey("UserDisplayName");
        public static readonly FieldKey UserEmail = new FieldKey("UserEmail");
        public static readonly FieldKey UserActive = new FieldKey("UserActive");
        public static readonly FieldKey UserAvatar = new FieldKey("UserAvatar");
        public static readonly FieldKey UserGroup = new FieldKey("UserGroup");

        public static readonly ImmutableArray<FieldKey> All = ImmutableArray.Create(
            UserAppId,
            UserCwdId,
            UserKey,
            UserName,
            UserDisplayName,
            UserEmail,
            UserActive,
            UserAvatar,
            UserGroup);
    }
}
