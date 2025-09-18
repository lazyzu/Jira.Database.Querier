using Generator.Equals;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project.Fields;

namespace lazyzu.Jira.Database.Querier.Project
{
    public interface IJiraProject
    {
        decimal Id { get; }
        string Name { get; }
        string Url { get; }
        User.IJiraUser Lead { get; }
        string Description { get; }
        string Key { get; }
        IProjectAvatar Avatar { get; }
        string Type { get; }
        IProjectCategory Category { get; }
        IProjectRoleActorMap[] ProjectRoles { get; }
        IIssueTypeScheme IssueTypeScheme { get; }
        IFullProjectComponent[] Components { get; }
        IFullProjectVersion[] Versions { get; }
        IIssueSecurityLevel[] SecurityLevels { get; }
    }

    [Equatable(Explicit = true)]
    public partial class JiraProject : IJiraProject
    {
        [DefaultEquality]
        public decimal Id { get; internal set; }
        public string Name { get; internal set; }
        public string Url { get; internal set; }
        public User.IJiraUser Lead { get; internal set; }
        public string Description { get; internal set; }
        public string Key { get; internal set; }
        public IProjectAvatar Avatar { get; internal set; }
        public string Type { get; internal set; }
        public IProjectCategory Category { get; internal set; }
        public IProjectRoleActorMap[] ProjectRoles { get; internal set; }
        public IIssueTypeScheme IssueTypeScheme { get; internal set; }
        public IFullProjectComponent[] Components { get; internal set; }
        public IFullProjectVersion[] Versions { get; internal set; }
        public IIssueSecurityLevel[] SecurityLevels { get; internal set; }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Key})";
        }
    }
}
