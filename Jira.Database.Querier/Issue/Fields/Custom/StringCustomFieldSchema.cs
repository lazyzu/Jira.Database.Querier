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
    public partial class StringCustomFieldSchema
    {
        [DefaultEquality]
        public string Value { get; init; }

        public int CompareTo(object obj)
        {
            if (obj is StringCustomFieldSchema other) return Value.CompareTo(other.Value);
            else if (obj is string otherString) return Value.CompareTo(otherString);
            else throw new NotSupportedException($"Not able to compare between {nameof(StringCustomFieldSchema)} & {obj.GetType().Name}");
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class StringCustomFieldProjection : IIssueCustomFieldProjectionSpecification
    {
        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public StringCustomFieldProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;
        }

        public virtual bool IsSupported(ICustomFieldKey customFieldKey)
        {
            var projectionType = customFieldKey.ProjectionType;
            return projectionType == typeof(StringCustomFieldSchema);
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
                            issue.CustomFields.TryAdd(customFieldKey, new StringCustomFieldSchema
                            {
                                Value = value
                            });
                        }
                    }
                }
            }
        }

        protected virtual async Task<Dictionary<decimal?, string>> LoadIssueValueMap(decimal?[] issueIds, decimal fieldTypeId, CancellationToken cancellationToken)
        {
            var query = jiraContext.customfieldvalue.AsNoTracking()
                .Where(customfieldvalue => issueIds.Contains(customfieldvalue.ISSUE)
                                        && fieldTypeId == customfieldvalue.CUSTOMFIELD)
                .Select(customfieldvalue => new
                {
                    customfieldvalue.ISSUE,
                    customfieldvalue.STRINGVALUE
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.ToDictionary(dbModel => dbModel.ISSUE
                                          , dbModel => dbModel.STRINGVALUE);
        }
    }

    public class StringCustomFieldSpecification : QuerySpecification<customfieldvalue>
    {
        public StringCustomFieldSpecification(ICustomFieldKey customFieldKey, Expression<Func<string, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((customfieldvalue customfieldvalue) => customfieldvalue.STRINGVALUE, predicate
                                                             , (customfieldvalue customfieldvalue) => customfieldvalue.CUSTOMFIELD == customFieldKey.Id);

            CriteriaGetter = () => Task.FromResult(criteria);
        }
    }
}
