using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier
{
    using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;

    public static class SelectOptionExtension
    {
        public static ISelectCollection AsCollection(this IEnumerable<ISelectOption> selectOptions)
        {
            return new SelectCollection
            {
                Selections = selectOptions?.ToHashSet() ?? new HashSet<ISelectOption>()
            };
        }

        public static ICascadingSelection AsCascading(this IEnumerable<ISelectOption> selectOptions)
        {
            return new CascadingSelection
            {
                CascadingSelections = selectOptions?.ToArray() ?? new ISelectOption[0]
            };
        }
    }
}

namespace lazyzu.Jira.Database.Querier.Issue.Fields.Custom
{
    public interface ISelectOption
    {
        decimal Id { get; }
        string Value { get; }
        bool Disabled { get; }
    }

    [Equatable(Explicit = true)]
    public partial class SelectOption : ISelectOption
    {
        [DefaultEquality]
        public decimal Id { get; init; }
        public string Value { get; init; }
        public bool Disabled { get; init; }

        public override string ToString()
        {
            return $"{Id} ({Value})";
        }
    }

    [Equatable(Explicit = true)]
    public partial class SelectCustomFieldSchema
    {
        [DefaultEquality]
        public ISelectOption Value { get; init; }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }

    public static class SelectCustomFieldExtension
    {
        public static async Task<(decimal IssueId, decimal OptionId)[]> LoadIssueOptionIdMap(decimal?[] issueIds, decimal fieldTypeId, JiraContext jiraContext, CancellationToken cancellationToken)
        {
            var query = jiraContext.customfieldvalue.AsNoTracking()
                            .Where(customfieldvalue => issueIds.Contains(customfieldvalue.ISSUE)
                                                    && customfieldvalue.CUSTOMFIELD == fieldTypeId)
                            .Select(customfieldvalue => new CustomfieldvalueDbModel(customfieldvalue.ISSUE, customfieldvalue.STRINGVALUE));

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var validIssueOptionMap = LoadValidIssueOptionMap(queryResult).ToArray();
            return validIssueOptionMap;
        }

        internal record CustomfieldvalueDbModel(decimal? ISSUE, string STRINGVALUE);

        internal static IEnumerable<(decimal IssueId, decimal OptionId)> LoadValidIssueOptionMap(CustomfieldvalueDbModel[] dbModels)
        {
            if (dbModels.Any())
            {
                foreach (var dbModel in dbModels)
                {
                    if (dbModel.ISSUE.HasValue && decimal.TryParse(dbModel.STRINGVALUE, out var optionId))
                    {
                        yield return (dbModel.ISSUE.Value, optionId);
                    }
                }
            }
        }

        public static async Task<Dictionary<decimal, SelectOption>> LoadSelectOptionMap(decimal[] optionIds, JiraContext jiraContext, CancellationToken cancellationToken)
        {
            var query = jiraContext.customfieldoption.AsNoTracking()
                .Where(customfieldoption => optionIds.Contains(customfieldoption.ID))
                .Select(customfieldoption => new
                {
                    customfieldoption.ID,
                    customfieldoption.customvalue,
                    customfieldoption.disabled
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.ToDictionary(dbModel => dbModel.ID
                                          , dbModel => new SelectOption
                                          {
                                              Id = dbModel.ID,
                                              Value = dbModel.customvalue,
                                              Disabled = IsDisabled(dbModel.disabled)
                                          });
        }

        public static bool IsDisabled(string strValue)
        {
            return "Y".Equals(strValue, StringComparison.OrdinalIgnoreCase);
        }
    }

    // com.atlassian.jira.plugin.system.customfieldtypes:select
    // com.atlassian.jira.plugin.system.customfieldtypes:radiobuttons

    public class SelectCustomFieldProjection : IIssueCustomFieldProjectionSpecification
    {
        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public SelectCustomFieldProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;
        }

        public virtual bool IsSupported(ICustomFieldKey customFieldKey)
        {
            var projectionType = customFieldKey.ProjectionType;
            return projectionType == typeof(SelectCustomFieldSchema);
        }

        public async Task Projection(ICustomFieldKey customFieldKey, IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueOptionMap = await LoadIssueValueMap(issueIds, customFieldKey.Id, cancellationToken).ConfigureAwait(false);

                if (issueOptionMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueOptionMap.TryGetValue(issue.Id, out var option))
                        {
                            issue.CustomFields.TryAdd(customFieldKey, new SelectCustomFieldSchema
                            {
                                Value = option
                            });
                        }
                    }
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, SelectOption>> LoadIssueValueMap(decimal?[] issueIds, decimal fieldTypeId, CancellationToken cancellationToken)
        {
            var validIssueOptionMap = await SelectCustomFieldExtension.LoadIssueOptionIdMap(issueIds, fieldTypeId, jiraContext, cancellationToken).ConfigureAwait(false);

            var optionIds = validIssueOptionMap.Select(map => map.OptionId).Distinct().ToArray();

            var selectOptionMap = await SelectCustomFieldExtension.LoadSelectOptionMap(optionIds, jiraContext, cancellationToken).ConfigureAwait(false);

            return validIssueOptionMap.GroupBy(map => map.IssueId)
                .ToDictionary(issueIdGroup => issueIdGroup.Key, issueIdGroup =>
                {
                    foreach (var map in issueIdGroup)
                    {
                        if (selectOptionMap.TryGetValue(map.OptionId, out var option)) return option;
                    }
                    return default;
                });
        }
    }

    public class SelectCustomFieldSpecification : QuerySpecification<customfieldvalue>
    {
        public SelectCustomFieldSpecification(ICustomFieldKey customFieldKey, Expression<Func<string, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((customfieldvalue customfieldvalue) => customfieldvalue.STRINGVALUE, predicate
                                                             , (customfieldvalue customfieldvalue) => customfieldvalue.CUSTOMFIELD == customFieldKey.Id);

            CriteriaGetter = () => Task.FromResult(criteria);
        }
    }
}
