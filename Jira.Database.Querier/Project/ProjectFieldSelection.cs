using lazyzu.Jira.Database.Querier.Project.Contract;
using System.Collections.Immutable;

namespace lazyzu.Jira.Database.Querier
{
    public static class ProjectFieldSelection
    {
        internal static readonly FieldKey ProjectId = new FieldKey("ProjectId");

        public static readonly FieldKey ProjectName = new FieldKey("ProjectName");
        public static readonly FieldKey ProjectUrl = new FieldKey("ProjectUrl");
        public static readonly FieldKey ProjectLead = new FieldKey("ProjectLead");
        public static FieldKey ProjectLeadWithField(User.Contract.FieldKey[] userFields)
            => new UserFieldKey()
            {
                Name = "ProjectLead",
                Fields = userFields ?? new User.Contract.FieldKey[]
                {
                    UserFieldSelection.UserName,
                    UserFieldSelection.UserKey
                }
            };

        public static readonly FieldKey ProjectDescription = new FieldKey("ProjectDescription");
        public static readonly FieldKey ProjectKey = new FieldKey("ProjectKey");
        public static readonly FieldKey ProjectAvatar = new FieldKey("ProjectAvatar");
        public static readonly FieldKey ProjectType = new FieldKey("ProjectType");
        public static readonly FieldKey ProjectCategory = new FieldKey("ProjectCategory");
        public static readonly FieldKey ProjectRole = new FieldKey("ProjectRole");
        public static readonly FieldKey ProjectIssueType = new FieldKey("ProjectIssueType");
        public static readonly FieldKey ProjectComponent = new FieldKey("ProjectComponent");
        public static readonly FieldKey ProjectVersion = new FieldKey("ProjectVersion");
        public static readonly FieldKey ProjectIssueSecurityLevel = new FieldKey("ProjectIssueSecurityLevel");

        public static readonly ImmutableArray<FieldKey> All = ImmutableArray.Create(
            ProjectName,
            ProjectUrl,
            ProjectLead,
            ProjectDescription,
            ProjectKey,
            ProjectAvatar,
            ProjectType,
            ProjectCategory,
            ProjectRole,
            ProjectIssueType,
            ProjectComponent,
            ProjectVersion,
            ProjectIssueSecurityLevel
        );

        public static ImmutableArray<FieldKey> AlleWithOption(FieldOption option)
            => ImmutableArray.Create(
            ProjectName,
            ProjectUrl,
            ProjectLeadWithField(option?.ProjectLeadFields),
            ProjectDescription,
            ProjectKey,
            ProjectAvatar,
            ProjectType,
            ProjectCategory,
            ProjectRole,
            ProjectIssueType,
            ProjectComponent,
            ProjectVersion,
            ProjectIssueSecurityLevel
        );

        public class FieldOption
        {
            public User.Contract.FieldKey[] ProjectLeadFields { get; set; } = null;
        }

        public class UserFieldKey : FieldKey, User.Contract.IUserFieldKeyCollection
        {
            public User.Contract.FieldKey[] Fields { get; init; }
        }
    }
}
