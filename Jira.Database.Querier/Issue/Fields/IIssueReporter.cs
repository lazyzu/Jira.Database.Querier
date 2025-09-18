using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using lazyzu.Jira.Database.Querier.User;
using lazyzu.Jira.Database.Querier.User.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueReporterProjection : IIssueProjectionWithContextSpecification<jiraissue>, IIssueProjectionSpecificationFieldKeyDependency
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        protected readonly Func<IUserService> userServiceGetter;

        public User.Contract.FieldKey[] UserKeys { get; protected init; } = null;

        public IssueReporterProjection(Func<IUserService> userServiceGetter)
        {
            this.userServiceGetter = userServiceGetter;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.Reporter
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.REPORTER
            };
        }

        public IssueReporterProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter) : this(() => jiraDatabaseQuerierGetter().User)
        { }

        public IIssueProjectionSpecification ConstructFrom(FieldKey fieldKey, FieldKey[] queryProjectionKeys)
        {
            if (fieldKey is User.Contract.IUserFieldKeyCollection userFieldKeyCollection) return new IssueReporterProjection(this.userServiceGetter)
            {
                UserKeys = UserFieldUtil.AddUserKeyFieldIfMissing(userFieldKeyCollection.Fields)
            };
            else return this;
        }

        public async Task<object> PrepareContext(IEnumerable<jiraissue> enties, CancellationToken cancellationToken = default)
        {
            var _enties = enties?.ToArray() ?? new jiraissue[0];

            if (_enties.Length == 0) return new Dictionary<string, IJiraUser>();

            var userKeys = _enties.Select(issue => issue.REPORTER)
                .Distinct()
                .ToArray();

            var userService = userServiceGetter();
            var users = await userService.GetUsersByKeyAsync(userKeys, fields: UserKeys ?? userService.DefaultQueryFields.ToArray(), cancellationToken);

            return users.ToDictionary(user => user.Key
                                    , user => user);
        }

        public virtual Task Projection(jiraissue entity, JiraIssue jiraIssue, object context, CancellationToken cancellationToken = default)
        {
            var map = context as IDictionary<string, IJiraUser>;

            if (entity.REPORTER != null && map.TryGetValue(entity.REPORTER, out var reporterUserInfo)) jiraIssue.Reporter = reporterUserInfo;
            return Task.CompletedTask;
        }
    }

    public class IssueReporterSpecification : QuerySpecification<jiraissue>
    {
        public IssueReporterSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.REPORTER, predicate));
        }
    }
}
