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
    [Equatable(Explicit = true)]
    public partial class NumberCustomFieldSchema : IComparable
    {
        [DefaultEquality]
        public decimal? Value { get; init; }

        public int CompareTo(object obj)
        {
            if (obj is NumberCustomFieldSchema other) return (Value ?? decimal.MinValue).CompareTo(other.Value ?? decimal.MinValue);
            else if (obj is decimal otherDecimal) return (Value ?? decimal.MinValue).CompareTo(otherDecimal);
            else throw new NotSupportedException($"Not able to compare between {nameof(NumberCustomFieldSchema)} & {obj.GetType().Name}");
        }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }

    public class NumberCustomFieldProjection : IIssueCustomFieldProjectionSpecification
    {
        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public NumberCustomFieldProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;
        }

        public virtual bool IsSupported(ICustomFieldKey customFieldKey)
        {
            var projectionType = customFieldKey.ProjectionType;
            return projectionType == typeof(NumberCustomFieldSchema);
        }

        public virtual async Task Projection(ICustomFieldKey customFieldKey, IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueValueMap = await LoadIssueValueMap(issueIds, customFieldKey.Id, cancellationToken).ConfigureAwait(false);

                if (issueValueMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueValueMap.TryGetValue(issue.Id, out var value))
                        {
                            issue.CustomFields.TryAdd(customFieldKey, new NumberCustomFieldSchema
                            {
                                Value = value
                            });
                        }
                    }
                }
            }
        }

        protected virtual async Task<Dictionary<decimal?, decimal?>> LoadIssueValueMap(decimal?[] issueIds, decimal fieldTypeId, CancellationToken cancellationToken)
        {
            var query = jiraContext.customfieldvalue.AsNoTracking()
                .Where(customfieldvalue => issueIds.Contains(customfieldvalue.ISSUE)
                                        && fieldTypeId == customfieldvalue.CUSTOMFIELD)
                .Select(customfieldvalue => new
                {
                    customfieldvalue.ISSUE,
                    customfieldvalue.NUMBERVALUE
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.ToDictionary(dbModel => dbModel.ISSUE
                                          , dbModel => dbModel.NUMBERVALUE);
        }
    }

    internal class NumberCustomFieldSpecification : QuerySpecification<customfieldvalue>
    {
        public NumberCustomFieldSpecification(ICustomFieldKey customFieldKey, Expression<Func<decimal?, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((customfieldvalue customfieldvalue) => customfieldvalue.NUMBERVALUE, predicate
                                                             , (customfieldvalue customfieldvalue) => customfieldvalue.CUSTOMFIELD == customFieldKey.Id);

            CriteriaGetter = () => Task.FromResult(criteria);
        }
    }
}
