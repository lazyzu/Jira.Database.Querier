using System.Linq;

namespace lazyzu.Jira.Database.Querier.User.Util
{
    internal static class UserFieldUtil
    {
        public static User.Contract.FieldKey[] AddUserKeyFieldIfMissing(User.Contract.FieldKey[] fields)
        {
            return fields.Concat(new User.Contract.FieldKey[]
            {
                UserFieldSelection.UserKey
            }).Distinct().ToArray();
        }
    }
}
