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
    public partial class LabelCustomFieldSchema
    {
        [SetEquality]
        public HashSet<string> Value { get; init; }

        public override string ToString()
        {
            if (Value == null) return string.Empty;
            return string.Join(", ", Value);
        }
    }

    public class LabelCustomFieldProjection : IIssueCustomFieldProjectionSpecification
    {
        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public LabelCustomFieldProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;
        }

        public virtual bool IsSupported(ICustomFieldKey customFieldKey)
        {
            var projectionType = customFieldKey.ProjectionType;
            return projectionType == typeof(LabelCustomFieldSchema);
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
                            issue.CustomFields.TryAdd(customFieldKey, new LabelCustomFieldSchema
                            {
                                Value = value
                            });
                        }
                    }
                }
            }
        }

        protected virtual async Task<Dictionary<decimal?, HashSet<string>>> LoadIssueValueMap(decimal?[] issueIds, decimal fieldTypeId, CancellationToken cancellationToken)
        {
            var query = jiraContext.label.AsNoTracking()
                .Where(label => issueIds.Contains(label.ISSUE)
                             && fieldTypeId == label.FIELDID)
                .Select(label => new
                {
                    label.ISSUE,
                    label.LABEL1
                });

            var queryResult = await query.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            return queryResult.GroupBy(dbModel => dbModel.ISSUE)
                .ToDictionary(issueIdGroup => issueIdGroup.Key
                            , issueIdGroup => issueIdGroup.Select(dbModel => dbModel.LABEL1).ToHashSet());
        }
    }

    public class LabelCustomFieldSpecification : QuerySpecification<label>
    {
        public LabelCustomFieldSpecification(ICustomFieldKey customFieldKey, Expression<Func<string, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((label customfieldvalue) => customfieldvalue.LABEL1, predicate
                                                             , (label customfieldvalue) => customfieldvalue.FIELDID == customFieldKey.Id);

            CriteriaGetter = () => Task.FromResult(criteria);
        }
    }
}
