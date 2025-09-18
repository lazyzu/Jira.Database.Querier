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

namespace lazyzu.Jira.Database.Querier.Issue.Fields.Custom
{
    public interface ICascadingSelection
    {
        ISelectOption[] CascadingSelections { get; }
    }

    // com.atlassian.jira.plugin.system.customfieldtypes:cascadingselect
    [Equatable(Explicit = true)]
    public partial class CascadingSelection : ICascadingSelection
    {
        [OrderedEquality]
        public ISelectOption[] CascadingSelections { get; init; }

        public override string ToString()
        {
            if (CascadingSelections == null) return string.Empty;
            return string.Join(" - ", CascadingSelections.Select(x => x.ToString()));
        }
    }

    [Equatable(Explicit = true)]
    public partial class CascadingSelectCustomFieldSchema
    {
        [DefaultEquality]
        public ICascadingSelection Value { get; init; }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }

    public class CascadingSelectCustomFieldProjection : IIssueCustomFieldProjectionSpecification
    {
        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public CascadingSelectCustomFieldProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;
        }

        public virtual bool IsSupported(ICustomFieldKey customFieldKey)
        {
            var projectionType = customFieldKey.ProjectionType;
            return projectionType == typeof(CascadingSelectCustomFieldSchema);
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
                            issue.CustomFields.TryAdd(customFieldKey, new CascadingSelectCustomFieldSchema
                            {
                                Value = option.AsCascading()
                            });
                        }
                    }
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, SelectOption[]>> LoadIssueValueMap(decimal?[] issueIds, decimal fieldTypeId, CancellationToken cancellationToken)
        {
            var validIssueOptionMap = await SelectCustomFieldExtension.LoadIssueOptionIdMap(issueIds, fieldTypeId, jiraContext, cancellationToken).ConfigureAwait(false);

            var optionIds = validIssueOptionMap.Select(map => map.OptionId).Distinct().ToArray();

            var selectOptionMap = await LoadCascadingOption(optionIds, cancellationToken).ConfigureAwait(false);

            return validIssueOptionMap.GroupBy(map => map.IssueId)
                .ToDictionary(issueIdGroup => issueIdGroup.Key
                            , issueIdGroup =>
                            {
                                foreach (var map in issueIdGroup)
                                {
                                    if (selectOptionMap.TryGetValue(map.OptionId, out var option)) return option;
                                }
                                return default;
                            });
        }

        internal protected virtual async Task<Dictionary<decimal, SelectOption[]>> LoadCascadingOption(decimal[] optionIds, CancellationToken cancellationToken)
        {
            if (optionIds.Any())
            {
                var queryContext = await LoadParentOption(optionIds, cancellationToken).ConfigureAwait(false);

                while (queryContext.ParentChildMap.Any())
                {
                    queryContext = await LoadParentOption(queryContext, cancellationToken).ConfigureAwait(false);
                }

                return queryContext.ResultCache
                    .ToDictionary(x => x.Key
                                , x => x.Value.Reverse<SelectOption>().ToArray());
            }
            else return new Dictionary<decimal, SelectOption[]>();
        }

        protected class QueryContext
        {
            public Dictionary<decimal, List<SelectOption>> ResultCache { get; init; }
            public Dictionary<decimal, decimal[]> ParentChildMap { get; init; }
        }

        protected virtual async Task<QueryContext> LoadParentOption(decimal[] optionIds, CancellationToken cancellationToken)
        {
            var query = jiraContext.customfieldoption.AsNoTracking()
                .Where(customfieldoption => optionIds.Contains(customfieldoption.ID))
                .Select(customfieldoption => new
                {
                    customfieldoption.ID,
                    customfieldoption.customvalue,
                    customfieldoption.PARENTOPTIONID,
                    customfieldoption.disabled
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return new QueryContext
            {
                ResultCache = queryResult.ToDictionary(dbModel => dbModel.ID
                                                     , dbModel => new List<SelectOption>
                                                     {
                                                         new SelectOption
                                                         {
                                                             Id = dbModel.ID,
                                                             Value = dbModel.customvalue,
                                                             Disabled = SelectCustomFieldExtension.IsDisabled(dbModel.disabled)
                                                         }
                                                     }),
                ParentChildMap = queryResult.Where(dbModel => dbModel.PARENTOPTIONID.HasValue)
                                            .GroupBy(dbModel => dbModel.PARENTOPTIONID.Value)
                                            .ToDictionary(parentIdGroup => parentIdGroup.Key
                                                        , parentIdGroup => parentIdGroup.Select(dbModel => dbModel.ID).ToArray())
            };
        }

        protected virtual async Task<QueryContext> LoadParentOption(QueryContext context, CancellationToken cancellationToken)
        {
            if (context.ParentChildMap.Any())
            {
                var optionIds = context.ParentChildMap.Keys;

                var query = jiraContext.customfieldoption.AsNoTracking()
                .Where(customfieldoption => optionIds.Contains(customfieldoption.ID))
                .Select(customfieldoption => new
                {
                    customfieldoption.ID,
                    customfieldoption.customvalue,
                    customfieldoption.PARENTOPTIONID,
                    customfieldoption.disabled
                });

                var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

                if (queryResult.Any())
                {
                    foreach (var dbModel in queryResult)
                    {
                        var option = new SelectOption
                        {
                            Id = dbModel.ID,
                            Value = dbModel.customvalue,
                            Disabled = SelectCustomFieldExtension.IsDisabled(dbModel.disabled)
                        };

                        if (context.ParentChildMap.TryGetValue(dbModel.ID, out var childIds))
                        {
                            foreach (var childId in childIds)
                            {
                                if (context.ResultCache.TryGetValue(childId, out var cache)) cache.Add(option);
                            }
                        }
                    }

                    return new QueryContext
                    {
                        ResultCache = context.ResultCache,
                        ParentChildMap = queryResult.Where(dbModel => dbModel.PARENTOPTIONID.HasValue)
                        .GroupBy(dbModel => dbModel.PARENTOPTIONID.Value)
                        .ToDictionary(parentIdGroup => parentIdGroup.Key
                                    , parentIdGroup =>
                                    {
                                        return parentIdGroup.SelectMany(dbModel =>
                                        {
                                            if (context.ParentChildMap.TryGetValue(dbModel.ID, out var childIds)) return childIds;
                                            else return new decimal[0];
                                        }).Distinct()
                                          .ToArray();
                                    })
                    };
                }
                else return new QueryContext
                {
                    ResultCache = context.ResultCache,
                    ParentChildMap = new Dictionary<decimal, decimal[]>()
                };
            }
            else return new QueryContext
            {
                ResultCache = context.ResultCache,
                ParentChildMap = new Dictionary<decimal, decimal[]>()
            };
        }
    }

    public class CascadingSelectCustomFieldSpecification : QuerySpecification<customfieldvalue>
    {
        public CascadingSelectCustomFieldSpecification(ICustomFieldKey customFieldKey, Expression<Func<string, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((customfieldvalue customfieldvalue) => customfieldvalue.STRINGVALUE, predicate
                                                             , (customfieldvalue customfieldvalue) => customfieldvalue.CUSTOMFIELD == customFieldKey.Id);

            CriteriaGetter = () => Task.FromResult(criteria);
        }
    }
}
