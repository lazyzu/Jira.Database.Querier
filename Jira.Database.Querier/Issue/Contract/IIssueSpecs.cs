using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;
using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using lazyzu.Jira.Database.Querier.User;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace lazyzu.Jira.Database.Querier.Issue.Contract
{
    public interface IIssueSpecs
    {
        IQuerySpecification IssueId(Expression<Func<decimal, bool>> predicate);
        IQuerySpecification IssueProjectId(Expression<Func<decimal?, bool>> predicate);
        IQuerySpecification IssueSummary(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueDescription(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueCreateDate(Expression<Func<DateTime?, bool>> predicate);
        IQuerySpecification IssueUpdateDate(Expression<Func<DateTime?, bool>> predicate);
        IQuerySpecification IssueDueDate(Expression<Func<DateTime?, bool>> predicate);
        IQuerySpecification IssueResolutionDate(Expression<Func<DateTime?, bool>> predicate);
        IQuerySpecification IssueSecurityLevelId(Expression<Func<decimal?, bool>> predicate);
        IQuerySpecification IssueAssignee(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueReporter(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueEnvironment(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueVotes(Expression<Func<decimal?, bool>> predicate);
        IQuerySpecification IssueStatusId(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueStatusCategoryId(Func<decimal, bool> predicate);
        IQuerySpecification IssuePriorityId(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueResolutionId(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueTypeId(Expression<Func<string, bool>> predicate);
        IQuerySpecification IssueAffectsVersions(Expression<Func<decimal?, bool>> predicate);
        IQuerySpecification IssueFixVersions(Expression<Func<decimal?, bool>> predicate);

        // DateTimeCustomField
        IQuerySpecification CustomField(CustomFieldKey<DateTimeCustomFieldSchema> field, Expression<Func<DateTime?, bool>> predicate);

        // LabelCustomField
        IQuerySpecification CustomField(CustomFieldKey<LabelCustomFieldSchema> field, Expression<Func<string, bool>> predicate);

        // NumberCustomField
        IQuerySpecification CustomField(CustomFieldKey<NumberCustomFieldSchema> field, Expression<Func<decimal?, bool>> predicate);

        // SelectCustomField
        IQuerySpecification CustomField(CustomFieldKey<SelectCustomFieldSchema> field, Expression<Func<string, bool>> predicate);

        // StringCustomField
        IQuerySpecification CustomField(CustomFieldKey<StringCustomFieldSchema> field, Expression<Func<string, bool>> predicate);

        // TextCustomField
        IQuerySpecification CustomField(CustomFieldKey<TextCustomFieldSchema> field, Expression<Func<string, bool>> predicate);

        // MultiSelectField
        IQuerySpecification CustomField(CustomFieldKey<MultiSelectCustomFieldSchema> field, Expression<Func<string, bool>> predicate);

        // CascadingSelectField
        IQuerySpecification CustomField(CustomFieldKey<CascadingSelectCustomFieldSchema> field, Expression<Func<string, bool>> predicate);

        // UserField
        IQuerySpecification CustomField(CustomFieldKey<UserCustomFieldSchema> field, Expression<Func<string, bool>> predicate);

        // MultiUserField
        IQuerySpecification CustomField(CustomFieldKey<MultiUserCustomFieldSchema> field, Expression<Func<string, bool>> predicate);
    }
}

namespace lazyzu.Jira.Database.Querier
{
    public static class IssueSpecExtension
    {
        public static IQuerySpecification IssueProject(this IIssueSpecs specs, params IJiraProject[] projects)
        {
            var targetProjectIds = projects.Select<IJiraProject, decimal?>(project => project.Id).ToArray();
            return specs.IssueProjectId(projectId => targetProjectIds.Contains(projectId));
        }

        public static IQuerySpecification IssueSecurityLevel(this IIssueSpecs specs, params IIssueSecurityLevel[] issueSecurityLevels)
        {
            var targetSecurityLevelIds = issueSecurityLevels.Select<IIssueSecurityLevel, decimal?>(issueSecurityLevel => issueSecurityLevel.Id).ToArray();
            return specs.IssueSecurityLevelId(issueSecurityLevelId => targetSecurityLevelIds.Contains(issueSecurityLevelId));
        }

        public static IQuerySpecification IssueAssignee(this IIssueSpecs specs, params IJiraUser[] jiraUsers)
        {
            var targetUserKeys = ToValidUserKeys(jiraUsers);
            return specs.IssueAssignee(assignee => targetUserKeys.Contains(assignee));
        }

        public static IQuerySpecification IssueReporter(this IIssueSpecs specs, params IJiraUser[] jiraUsers)
        {
            var targetUserKeys = ToValidUserKeys(jiraUsers);
            return specs.IssueReporter(assignee => targetUserKeys.Contains(assignee));
        }

        public static IQuerySpecification IssueStatus(this IIssueSpecs specs, params IIssueStatus[] issueStatuses)
        {
            var targetStatusIds = issueStatuses.Select(issueStatus => issueStatus.Id).ToArray();
            return specs.IssueStatusId(statusId => targetStatusIds.Contains(statusId));
        }

        public static IQuerySpecification IssueStatusCategory(this IIssueSpecs specs, params IIssueStatusCategory[] issueStatusCategories)
        {
            var targetStatusCategoryIds = issueStatusCategories.Select(issueStatusCategory => issueStatusCategory.Id).ToArray();
            return specs.IssueStatusCategoryId(statusCategoryId => targetStatusCategoryIds.Contains(statusCategoryId));
        }

        public static IQuerySpecification IssuePriority(this IIssueSpecs specs, params IIssuePriority[] issuePriorities)
        {
            var targetPriorityIds = issuePriorities.Select(issuePriority => issuePriority.Id).ToArray();
            return specs.IssuePriorityId(priorityId => targetPriorityIds.Contains(priorityId));
        }

        public static IQuerySpecification IssueResolution(this IIssueSpecs specs, params IIssueResolution[] issueResolutions)
        {
            var targetResolutionIds = issueResolutions.Select(issueResolution => issueResolution.Id).ToArray();
            return specs.IssueResolutionId(resolutionId => targetResolutionIds.Contains(resolutionId));
        }

        public static IQuerySpecification IssueType(this IIssueSpecs specs, params IIssueType[] issueTypes)
        {
            var targetTypeIds = issueTypes.Select(issueType => issueType.Id).ToArray();
            return specs.IssueTypeId(typeId => targetTypeIds.Contains(typeId));
        }

        public static IQuerySpecification IssueAffectsVersions(this IIssueSpecs specs, params IProjectVersion[] versions)
        {
            var targetVersionIds = versions.Select<IProjectVersion, decimal?>(version => version.Id).ToArray();
            return specs.IssueAffectsVersions(versionId => targetVersionIds.Contains(versionId));
        }

        public static IQuerySpecification IssueFixVersions(this IIssueSpecs specs, params IProjectVersion[] versions)
        {
            var targetVersionIds = versions.Select<IProjectVersion, decimal?>(version => version.Id).ToArray();
            return specs.IssueFixVersions(versionId => targetVersionIds.Contains(versionId));
        }

        private static ImmutableArray<string> ToValidUserKeys(IJiraUser[] jiraUsers)
        {
            return jiraUsers.Select(user => user.Key)
                .Where(userKey => string.IsNullOrEmpty(userKey) == false)
                .ToImmutableArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="field"></param>
        /// <param name="labels">allowed labels (only support or, not support and)</param>
        /// <returns></returns>
        public static IQuerySpecification CustomField(this IIssueSpecs specs, CustomFieldKey<LabelCustomFieldSchema> field, params string[] labels)
        {
            return specs.CustomField(field, (string label) => labels.Contains(label));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="field"></param>
        /// <param name="selections">allowed selections (only support or, not support and)</param>
        /// <returns></returns>
        public static IQuerySpecification CustomField(this IIssueSpecs specs, CustomFieldKey<SelectCustomFieldSchema> field, params ISelectOption[] selections)
        {
            var targetSelectIds = selections.Select(select => select.Id.ToString());
            return specs.CustomField(field, (string selectionId) => targetSelectIds.Contains(selectionId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="field"></param>
        /// <param name="selections">allowed selections (only support or, not support and)</param>
        /// <returns></returns>
        public static IQuerySpecification CustomField(this IIssueSpecs specs, CustomFieldKey<MultiSelectCustomFieldSchema> field, params ISelectOption[] selections)
        {
            var targetSelectIds = selections.Select(select => select.Id.ToString());
            return specs.CustomField(field, (string selectionId) => targetSelectIds.Contains(selectionId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="field"></param>
        /// <param name="selections">allowed selections (only support or, not support and)</param>
        /// <returns></returns>
        public static IQuerySpecification CustomField(this IIssueSpecs specs, CustomFieldKey<CascadingSelectCustomFieldSchema> field, params ICascadingSelection[] selections)
        {
            var targetSelectIds = selections.Select(select => select.CascadingSelections.Last().Id.ToString());
            return specs.CustomField(field, (string selectionId) => targetSelectIds.Contains(selectionId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="field"></param>
        /// <param name="selections">allowed users (only support or, not support and)</param>
        /// <returns></returns>
        public static IQuerySpecification CustomField(this IIssueSpecs specs, CustomFieldKey<UserCustomFieldSchema> field, params IJiraUser[] users)
        {
            var targetUserKeys = users.Select(user => user.Key)
                .Where(userKey => string.IsNullOrEmpty(userKey) == false)
                .ToArray();

            return specs.CustomField(field, (string selectionId) => targetUserKeys.Contains(selectionId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="field"></param>
        /// <param name="selections">allowed users (only support or, not support and)</param>
        /// <returns></returns>
        public static IQuerySpecification CustomField(this IIssueSpecs specs, CustomFieldKey<MultiUserCustomFieldSchema> field, params IJiraUser[] users)
        {
            var targetUserKeys = users.Select(user => user.Key)
                .Where(userKey => string.IsNullOrEmpty(userKey) == false)
                .ToArray();

            return specs.CustomField(field, (string selectionId) => targetUserKeys.Contains(selectionId));
        }
    }
}

namespace lazyzu.Jira.Database.Querier.Issue
{
    public class IssueSpecs : IIssueSpecs
    {
        private readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;

        public IssueSpecs(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter)
        {
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
        }

        public IQuerySpecification IssueId(Expression<Func<decimal, bool>> predicate)
            => new IssueIdSpecification(predicate);

        public IQuerySpecification IssueProjectId(Expression<Func<decimal?, bool>> predicate)
            => new IssueProjectIdSpecification(predicate);

        public IQuerySpecification IssueSummary(Expression<Func<string, bool>> predicate)
            => new IssueSummarySpecification(predicate);

        public IQuerySpecification IssueDescription(Expression<Func<string, bool>> predicate)
            => new IssueDescriptionSpecification(predicate);

        public IQuerySpecification IssueCreateDate(Expression<Func<DateTime?, bool>> predicate)
            => new IssueCreateDateSpecification(predicate);

        public IQuerySpecification IssueUpdateDate(Expression<Func<DateTime?, bool>> predicate)
            => new IssueUpdateDateSpecification(predicate);

        public IQuerySpecification IssueDueDate(Expression<Func<DateTime?, bool>> predicate)
             => new IssueDueDateSpecification(predicate);

        public IQuerySpecification IssueResolutionDate(Expression<Func<DateTime?, bool>> predicate)
            => new IssueResolutionDateSpecification(predicate);

        public IQuerySpecification IssueSecurityLevelId(Expression<Func<decimal?, bool>> predicate)
             => new IssueSecurityLevelIdSpecification(predicate);

        public IQuerySpecification IssueAssignee(Expression<Func<string, bool>> predicate)
            => new IssueAssigneeSpecification(predicate);

        public IQuerySpecification IssueReporter(Expression<Func<string, bool>> predicate)
            => new IssueReporterSpecification(predicate);

        public IQuerySpecification IssueEnvironment(Expression<Func<string, bool>> predicate)
            => new IssueEnvironmentSpecification(predicate);

        public IQuerySpecification IssueVotes(Expression<Func<decimal?, bool>> predicate)
            => new IssueVotesSpecification(predicate);

        public IQuerySpecification IssueStatusId(Expression<Func<string, bool>> predicate)
             => new IssueStatusIdSpecification(predicate);

        public IQuerySpecification IssueStatusCategoryId(Func<decimal, bool> predicate)
             => new IssueStatusCategoryIdSpecification(jiraDatabaseQuerierGetter, predicate);

        public IQuerySpecification IssuePriorityId(Expression<Func<string, bool>> predicate)
            => new IssuePriorityIdSpecification(predicate);

        public IQuerySpecification IssueResolutionId(Expression<Func<string, bool>> predicate)
            => new IssueResolutionIdSpecification(predicate);

        public IQuerySpecification IssueTypeId(Expression<Func<string, bool>> predicate)
            => new IssueTypeIdSpecification(predicate);

        public IQuerySpecification IssueAffectsVersions(Expression<Func<decimal?, bool>> predicate)
            => new IssueAffectsVersionsSpecification(predicate);

        public IQuerySpecification IssueFixVersions(Expression<Func<decimal?, bool>> predicate)
            => new IssueFixVersionsSpecification(predicate);

        // DateTimeCustomField
        public IQuerySpecification CustomField(CustomFieldKey<DateTimeCustomFieldSchema> field, Expression<Func<DateTime?, bool>> predicate)
            => new DateTimeCustomFieldSpecification(field, predicate);

        // LabelCustomField
        public IQuerySpecification CustomField(CustomFieldKey<LabelCustomFieldSchema> field, Expression<Func<string, bool>> predicate)
            => new LabelCustomFieldSpecification(field, predicate);

        // NumberCustomField
        public IQuerySpecification CustomField(CustomFieldKey<NumberCustomFieldSchema> field, Expression<Func<decimal?, bool>> predicate)
            => new NumberCustomFieldSpecification(field, predicate);

        // SelectCustomField
        public IQuerySpecification CustomField(CustomFieldKey<SelectCustomFieldSchema> field, Expression<Func<string, bool>> predicate)
            => new SelectCustomFieldSpecification(field, predicate);

        // StringCustomField
        public IQuerySpecification CustomField(CustomFieldKey<StringCustomFieldSchema> field, Expression<Func<string, bool>> predicate)
            => new StringCustomFieldSpecification(field, predicate);

        // TextCustomField
        public IQuerySpecification CustomField(CustomFieldKey<TextCustomFieldSchema> field, Expression<Func<string, bool>> predicate)
            => new TextCustomFieldSpecification(field, predicate);

        // MultiSelectCustomField
        public IQuerySpecification CustomField(CustomFieldKey<MultiSelectCustomFieldSchema> field, Expression<Func<string, bool>> predicate)
            => new MultiSelectCustomFieldSpecification(field, predicate);

        // CascadingSelectCustomField
        public IQuerySpecification CustomField(CustomFieldKey<CascadingSelectCustomFieldSchema> field, Expression<Func<string, bool>> predicate)
            => new CascadingSelectCustomFieldSpecification(field, predicate);

        // UserCustomField
        public IQuerySpecification CustomField(CustomFieldKey<UserCustomFieldSchema> field, Expression<Func<string, bool>> predicate)
            => new UserCustomFieldSpecification(field, predicate);

        // MultiUserCustomField
        public IQuerySpecification CustomField(CustomFieldKey<MultiUserCustomFieldSchema> field, Expression<Func<string, bool>> predicate)
            => new MultiUserCustomFieldSpecification(field, predicate);
    }
}
