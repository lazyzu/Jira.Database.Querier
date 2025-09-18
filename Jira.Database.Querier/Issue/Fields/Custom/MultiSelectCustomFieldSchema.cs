using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields.Custom
{
    public interface ISelectCollection
    {
        HashSet<ISelectOption> Selections { get; }
    }

    [Equatable(Explicit = true)]
    public partial class SelectCollection : ISelectCollection
    {
        [SetEquality]
        public HashSet<ISelectOption> Selections { get; init; }

        public override string ToString()
        {
            if (Selections == null) return string.Empty;
            return string.Join(", ", Selections.Select(x => x.ToString()));
        }
    }

    // com.atlassian.jira.plugin.system.customfieldtypes:multiselect
    // com.atlassian.jira.plugin.system.customfieldtypes:multicheckboxes
    [Equatable(Explicit = true)]
    public partial class MultiSelectCustomFieldSchema
    {
        [DefaultEquality]
        public ISelectCollection Value { get; init; }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }

    public class MultiSelectCustomFieldProjection : IIssueCustomFieldProjectionSpecification
    {
        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public MultiSelectCustomFieldProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;
        }

        public virtual bool IsSupported(ICustomFieldKey customFieldKey)
        {
            var projectionType = customFieldKey.ProjectionType;
            return projectionType == typeof(MultiSelectCustomFieldSchema);
        }

        public async Task Projection(ICustomFieldKey customFieldKey, IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueOptionsMap = await LoadIssueValueMap(issueIds, customFieldKey.Id, cancellationToken).ConfigureAwait(false);

                if (issueOptionsMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueOptionsMap.TryGetValue(issue.Id, out var options))
                        {
                            issue.CustomFields.TryAdd(customFieldKey, new MultiSelectCustomFieldSchema
                            {
                                Value = options.AsCollection()
                            });
                        }
                    }
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, HashSet<ISelectOption>>> LoadIssueValueMap(decimal?[] issueIds, decimal fieldTypeId, CancellationToken cancellationToken)
        {
            var validIssueOptionMap = await SelectCustomFieldExtension.LoadIssueOptionIdMap(issueIds, fieldTypeId, jiraContext, cancellationToken).ConfigureAwait(false);

            var optionIds = validIssueOptionMap.Select(map => map.OptionId).Distinct().ToArray();

            var selectOptionMap = await SelectCustomFieldExtension.LoadSelectOptionMap(optionIds, jiraContext, cancellationToken).ConfigureAwait(false);

            return validIssueOptionMap.GroupBy(map => map.IssueId)
                .ToDictionary(issueIdGroup => issueIdGroup.Key, issueIdGroup =>
                {
                    return LoadOptions(issueIdGroup, selectOptionMap).ToHashSet();
                });
        }

        protected IEnumerable<ISelectOption> LoadOptions(IEnumerable<(decimal IssueId, decimal OptionId)> issueOptionMap, Dictionary<decimal, SelectOption> selectOptionMap)
        {
            foreach (var x in issueOptionMap)
            {
                if (selectOptionMap.TryGetValue(x.OptionId, out var option)) yield return option;
            }
        }
    }

    public class MultiSelectCustomFieldSpecification : QuerySpecification<customfieldvalue>
    {
        public MultiSelectCustomFieldSpecification(ICustomFieldKey customFieldKey, Expression<Func<string, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((customfieldvalue customfieldvalue) => customfieldvalue.STRINGVALUE, predicate
                                                             , (customfieldvalue customfieldvalue) => customfieldvalue.CUSTOMFIELD == customFieldKey.Id);

            CriteriaGetter = () => Task.FromResult(criteria);
        }
    }
}
