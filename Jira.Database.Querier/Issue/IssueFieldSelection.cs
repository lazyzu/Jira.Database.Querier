using lazyzu.Jira.Database.Querier.Issue.Contract;
using System.Collections.Immutable;

namespace lazyzu.Jira.Database.Querier
{
    public static class IssueFieldSelection
    {
        internal static readonly FieldKey IssueId = new FieldKey("id");

        public static readonly FieldKey IssueNum = new FieldKey("num");
        public static readonly FieldKey Project = new FieldKey("project");
        public static FieldKey ProjectWithField(Project.Contract.FieldKey[] projectFields)
            => new ProjectFieldKey()
            {
                Name = "project",
                Fields = projectFields ?? new Project.Contract.FieldKey[]
                {
                    ProjectFieldSelection.ProjectKey,
                    ProjectFieldSelection.ProjectName
                }
            };

        public static readonly FieldKey Key = new FieldKey("key");
        public static readonly FieldKey Summary = new FieldKey("summary");
        public static readonly FieldKey Description = new FieldKey("description");
        public static readonly FieldKey CreateDate = new FieldKey("created");
        public static readonly FieldKey UpdateDate = new FieldKey("updated");
        public static readonly FieldKey DueDate = new FieldKey("duedate");
        public static readonly FieldKey ResolutionDate = new FieldKey("resolutiondate");
        public static readonly FieldKey SecurityLevel = new FieldKey("security");
        public static readonly FieldKey Assignee = new FieldKey("assignee");
        public static FieldKey AssigneeWithField(User.Contract.FieldKey[] userFields)
            => new UserFieldKey()
            {
                Name = "assignee",
                Fields = userFields ?? new User.Contract.FieldKey[]
                {
                    UserFieldSelection.UserName,
                    UserFieldSelection.UserKey
                }
            };

        public static readonly FieldKey Reporter = new FieldKey("reporter");
        public static FieldKey ReporterWithField(User.Contract.FieldKey[] userFields)
            => new UserFieldKey()
            {
                Name = "reporter",
                Fields = userFields ?? new User.Contract.FieldKey[]
                {
                    UserFieldSelection.UserName,
                    UserFieldSelection.UserKey
                }
            };

        public static readonly FieldKey Environment = new FieldKey("Environment");
        public static readonly FieldKey Votes = new FieldKey("votes");
        public static readonly FieldKey IssueStatus = new FieldKey("status");
        public static readonly FieldKey Priority = new FieldKey("priority");
        public static readonly FieldKey Resolution = new FieldKey("resolution");
        public static readonly FieldKey IssueType = new FieldKey("issuetype");
        public static readonly FieldKey Components = new FieldKey("components");
        public static readonly FieldKey AffectsVersions = new FieldKey("versions");
        public static readonly FieldKey FixVersions = new FieldKey("fixVersions");
        public static readonly FieldKey Labels = new FieldKey("labels");
        public static readonly FieldKey Comments = new FieldKey("comment");
        public static readonly FieldKey Worklogs = new FieldKey("worklog");
        public static readonly FieldKey Changelogs = new FieldKey("changelog");
        public static readonly FieldKey ParentIssueId = new FieldKey("parent");
        public static readonly FieldKey SubTaskIds = new FieldKey("subtasks");
        public static readonly FieldKey IssueLinks = new FieldKey("issuelinks");
        public static readonly FieldKey RemoteLinks = new FieldKey("RemoteLinks");
        public static readonly FieldKey Attachments = new FieldKey("attachment");

        public static readonly ImmutableArray<FieldKey> AllNative = ImmutableArray.Create(
            IssueNum,
            Project,
            Key,
            Summary,
            Description,
            CreateDate,
            UpdateDate,
            DueDate,
            ResolutionDate,
            SecurityLevel,
            Assignee,
            Reporter,
            Environment,
            Votes,
            IssueStatus,
            Priority,
            Resolution,
            IssueType,
            Components,
            AffectsVersions,
            FixVersions,
            Labels,
            Comments,
            Worklogs,
            Changelogs,
            ParentIssueId,
            SubTaskIds,
            IssueLinks,
            RemoteLinks,
            Attachments
        );

        public static ImmutableArray<FieldKey> AllNativeWithOption(FieldOption option)
            => ImmutableArray.Create(
            IssueNum,
            ProjectWithField(option?.ProjectFields),
            Key,
            Summary,
            Description,
            CreateDate,
            UpdateDate,
            DueDate,
            ResolutionDate,
            SecurityLevel,
            AssigneeWithField(option?.AssigneeFields),
            ReporterWithField(option?.ReporterFields),
            Environment,
            Votes,
            IssueStatus,
            Priority,
            Resolution,
            IssueType,
            Components,
            AffectsVersions,
            FixVersions,
            Labels,
            Comments,
            Worklogs,
            Changelogs,
            ParentIssueId,
            SubTaskIds,
            IssueLinks,
            RemoteLinks,
            Attachments
        );

        public class FieldOption
        {
            public Project.Contract.FieldKey[] ProjectFields { get; set; } = null;
            public User.Contract.FieldKey[] AssigneeFields { get; set; } = null;
            public User.Contract.FieldKey[] ReporterFields { get; set; } = null;
        }

        public class UserFieldKey : FieldKey, User.Contract.IUserFieldKeyCollection
        {
            public User.Contract.FieldKey[] Fields { get; init; }
        }

        public class ProjectFieldKey : FieldKey, Project.Contract.IProjectFieldKeyCollection
        {
            public Project.Contract.FieldKey[] Fields { get; init; }
        }
    }
}
