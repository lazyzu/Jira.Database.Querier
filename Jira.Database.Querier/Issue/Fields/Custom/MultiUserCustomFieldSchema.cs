using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using lazyzu.Jira.Database.Querier.User;
using lazyzu.Jira.Database.Querier.User.Util;
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
    public partial class MultiUserCustomFieldSchema
    {
        [DefaultEquality]
        public IJiraUserCollection Value { get; init; }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }

    public class MultiUserCustomFieldProjection : IIssueCustomFieldProjectionSpecification, IIssueCustomFieldProjectionSpecificationFieldKeyDependency
    {
        protected readonly JiraContext jiraContext;
        protected readonly Func<IUserService> userServiceGetter;
        protected readonly ILogger logger;

        public User.Contract.FieldKey[] UserKeys { get; protected init; } = null;

        public MultiUserCustomFieldProjection(JiraContext jiraContext, Func<IUserService> userServiceGetter, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.userServiceGetter = userServiceGetter;
            this.logger = logger;
        }

        public MultiUserCustomFieldProjection(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, ILogger logger)
            : this(jiraContext, () => jiraDatabaseQuerierGetter().User, logger)
        { }

        public virtual bool IsSupported(ICustomFieldKey customFieldKey)
        {
            var projectionType = customFieldKey.ProjectionType;
            return projectionType == typeof(MultiUserCustomFieldSchema);
        }

        public IIssueCustomFieldProjectionSpecification ConstructFrom(ICustomFieldKey fieldKey)
        {
            if (fieldKey is User.Contract.IUserFieldKeyCollection userFieldKeyCollection) return new MultiUserCustomFieldProjection(jiraContext, userServiceGetter, logger)
            {
                UserKeys = UserFieldUtil.AddUserKeyFieldIfMissing(userFieldKeyCollection.Fields)
            };
            else return this;
        }

        public async Task Projection(ICustomFieldKey customFieldKey, IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
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
                            issue.CustomFields.TryAdd(customFieldKey, new MultiUserCustomFieldSchema
                            {
                                Value = value.AsCollection()
                            });
                        }
                    }
                }
            }
        }

        protected virtual async Task<Dictionary<decimal?, HashSet<IJiraUser>>> LoadIssueValueMap(decimal?[] issueIds, decimal fieldTypeId, CancellationToken cancellationToken)
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

            if (queryResult.Any())
            {
                var userKeys = queryResult.Select(dbModel => dbModel.STRINGVALUE).Distinct().ToArray();

                var userService = userServiceGetter();
                var userInfoMap = (await userService.GetUsersByKeyAsync(userKeys, UserKeys ?? userService.DefaultQueryFields.ToArray(), cancellationToken).ConfigureAwait(false))
                    .ToDictionary(user => user.Key?.Trim() ?? string.Empty
                                , user => user);

                return queryResult.GroupBy(dbModel => dbModel.ISSUE)
                    .ToDictionary(issueIdGroup => issueIdGroup.Key
                                , issueIdGroup =>
                                {
                                    return LoadUsers(issueIdGroup.Select(dbModel => dbModel.STRINGVALUE).Distinct(), userInfoMap).ToHashSet();
                                });
            }
            else return new Dictionary<decimal?, HashSet<IJiraUser>>();
        }

        protected IEnumerable<IJiraUser> LoadUsers(IEnumerable<string> userNames, Dictionary<string, IJiraUser> userMap)
        {
            foreach (var x in userNames)
            {
                if (userMap.TryGetValue(x, out var user)) yield return user;
            }
        }
    }

    public class MultiUserCustomFieldSpecification : QuerySpecification<customfieldvalue>
    {
        public MultiUserCustomFieldSpecification(ICustomFieldKey customFieldKey, Expression<Func<string, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((customfieldvalue customfieldvalue) => customfieldvalue.STRINGVALUE, predicate
                                                             , (customfieldvalue customfieldvalue) => customfieldvalue.CUSTOMFIELD == customFieldKey.Id);

            CriteriaGetter = () => Task.FromResult(criteria);
        }
    }
}
