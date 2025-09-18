using Generator.Equals;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.User;
using System;
using System.Collections.Generic;

namespace lazyzu.Jira.Database.Querier.Issue
{
    public interface IJiraIssue : IComparable<IJiraIssue>
    {
        decimal Id { get; }
        decimal? IssueNum { get; }
        Project.IJiraProject Project { get; }
        string Key { get; }
        string Summary { get; }
        string Description { get; }
        DateTime? CreateDate { get; }
        DateTime? UpdateDate { get; }
        DateTime? DueDate { get; }
        DateTime? ResolutionDate { get; }
        IIssueSecurityLevel SecurityLevel { get; }

        IJiraUser Assignee { get; }
        IJiraUser Reporter { get; }
        string Environment { get; }
        decimal? Votes { get; }
        CustomFieldValueCollection CustomFields { get; }
        
        IIssueStatus IssueStatus { get; }
        IIssuePriority Priority { get; }
        IIssueResolution Resolution { get; }
        IIssueType IssueType { get; }

        IProjectComponent[] Components { get; }
        IProjectVersion[] AffectsVersions { get; }
        IProjectVersion[] FixVersions { get; }

        string[] Labels { get; }

        IIssueComment[] Comments { get; }
        IIssueWorklog[] Worklogs { get; }
        IIssueChangelog[] Changelogs { get; }

        decimal? ParentIssueId { get; }
        decimal[] SubTaskIds { get; }
        IIssueLink[] IssueLinks { get; }
        IIssueRemoteLink[] RemoteLinks { get; }
        IIssueAttachment[] Attachments { get; }
    }

    public class CustomFieldValueCollection
    {
        internal Dictionary<ICustomFieldKey, object> customFieldValueBag = new Dictionary<ICustomFieldKey, object>();

        public IEnumerable<ICustomFieldKey> Keys => customFieldValueBag.Keys;

        public int Count => customFieldValueBag.Count;

        public void Add(ICustomFieldKey key, object value)
            => customFieldValueBag.Add(key, value);

        public bool TryAdd(ICustomFieldKey key, object fieldValue)
        {
            if (customFieldValueBag.TryAdd(key, fieldValue)) return true;
            return false;
        }

        public void Clear()
            => customFieldValueBag.Clear();

        public bool ContainField(ICustomFieldKey key)
        => customFieldValueBag.ContainsKey(key);

        public bool TryGetValue<TCustomFieldValue>(CustomFieldKey<TCustomFieldValue> key, out TCustomFieldValue fieldValue)
        {
            if (customFieldValueBag.TryGetValue(key, out var valueObject))
            {
                if (valueObject is TCustomFieldValue customFieldValue)
                {
                    fieldValue = customFieldValue;
                    return true;
                }
            }

            fieldValue = default;
            return false;
        }

        public TCustomFieldValue GetValue<TCustomFieldValue>(CustomFieldKey<TCustomFieldValue> key, TCustomFieldValue defaultValue = default)
        {
            if (TryGetValue(key, out var result)) return result;
            else return defaultValue;
        }
    }

    [Equatable(Explicit = true)]
    public partial class JiraIssue : IJiraIssue
    {
        [DefaultEquality]
        public decimal Id { get; internal set; }
        public decimal? IssueNum { get; internal set; }
        public Project.IJiraProject Project { get; internal set; }
        public string Key
        {
            get
            {
                if (IssueNum.HasValue && !string.IsNullOrEmpty(Project?.Key)) return $"{Project.Key}-{IssueNum}";
                else return null;
            }
        }
        public string Summary { get; internal set; }
        public string Description { get; internal set; }
        public DateTime? CreateDate { get; internal set; }
        public DateTime? UpdateDate { get; internal set; }
        public DateTime? DueDate { get; internal set; }
        public DateTime? ResolutionDate { get; internal set; }
        public IIssueSecurityLevel SecurityLevel { get; internal set; }

        public IJiraUser Assignee { get; internal set; }
        public IJiraUser Reporter { get; internal set; }
        public string Environment { get; internal set; }
        public decimal? Votes { get; internal set; }
        public CustomFieldValueCollection CustomFields { get; internal set; } = new CustomFieldValueCollection();

        public IIssueStatus IssueStatus { get; internal set; }
        public IIssuePriority Priority { get; internal set; }
        public IIssueResolution Resolution { get; internal set; }
        public IIssueType IssueType { get; internal set; }
        public IProjectComponent[] Components { get; internal set; }
        public IProjectVersion[] AffectsVersions { get; internal set; }
        public IProjectVersion[] FixVersions { get; internal set; }
        public string[] Labels { get; internal set; }

        public IIssueComment[] Comments { get; internal set; }
        public IIssueWorklog[] Worklogs { get; internal set; }
        public IIssueChangelog[] Changelogs { get; internal set; }

        public decimal? ParentIssueId { get; internal set; }
        public decimal[] SubTaskIds { get; internal set; }
        public IIssueLink[] IssueLinks { get; internal set; }
        public IIssueRemoteLink[] RemoteLinks { get; internal set; }
        public IIssueAttachment[] Attachments { get; internal set; }

        public int CompareTo(IJiraIssue other)
        {
            return Id.CompareTo(other.Id);
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
