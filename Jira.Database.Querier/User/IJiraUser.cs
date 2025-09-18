using Generator.Equals;
using lazyzu.Jira.Database.Querier.User.Fields;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.User
{
    public interface IJiraUser
    {
        decimal AppId { get; }
        decimal CwdId { get; }
        string Key { get; }
        string Username { get; }
        string DisplayName { get; }
        string Email { get; }
        bool? IsActive { get; }
        IUserAvatar Avatar { get; }
        HashSet<IUserGroup> Groups { get; }
    }

    [Equatable(Explicit = true)]
    public partial class JiraUser : IJiraUser
    {
        public decimal AppId { get; internal set; }

        public decimal CwdId { get; internal set; }

        [DefaultEquality]
        public string Key { get; internal set; }

        [DefaultEquality]
        public string Username { get; internal set; }

        public string DisplayName { get; internal set; }

        public string Email { get; internal set; }

        public bool? IsActive { get; internal set; }

        public IUserAvatar Avatar { get; internal set; }

        public HashSet<IUserGroup> Groups { get; internal set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Username) == false) return Username;
            if (string.IsNullOrEmpty(Key) == false) return Key;
            return string.Empty;
        }
    }

    public interface IJiraUserCollection
    {
        HashSet<IJiraUser> Users { get; }
    }

    [Equatable(Explicit = true)]
    public partial class JiraUserCollection : IJiraUserCollection
    {
        [SetEquality]
        public HashSet<IJiraUser> Users { get; init; }

        public override string ToString()
        {
            if (Users == null) return string.Empty;
            return string.Join(", ", Users.Select(x => x.ToString()));
        }
    }
}

namespace lazyzu.Jira.Database.Querier
{
    using lazyzu.Jira.Database.Querier.User;

    public static class JiraUserExtension
    {
        public static IJiraUserCollection AsCollection(this IEnumerable<IJiraUser> users)
        {
            return new JiraUserCollection
            {
                Users = users?.ToHashSet() ?? new HashSet<IJiraUser>()
            };
        }
    }
}
