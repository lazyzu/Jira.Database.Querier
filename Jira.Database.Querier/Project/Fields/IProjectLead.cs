using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Project.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using lazyzu.Jira.Database.Querier.User;
using lazyzu.Jira.Database.Querier.User.Util;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Fields
{
    public class ProjectLeadProjection : IProjectProjectionWithContextSpecification<project>, IProjectProjectionSpecificationFieldKeyDependency
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<project, object>>[] IncludeExpressions { get; private init; }

        protected readonly Func<IUserService> userServiceGetter;

        public User.Contract.FieldKey[] UserKeys { get; protected init; } = null;

        public ProjectLeadProjection(Func<IUserService> userServiceGetter)
        {
            this.userServiceGetter = userServiceGetter;

            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectLead
            };

            IncludeExpressions = new Expression<Func<project, object>>[]
            {
                project => project.LEAD
            };
        }

        public ProjectLeadProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter) : this(() => jiraDatabaseQuerierGetter().User)
        { }

        public IProjectProjectionSpecification ConstructFrom(FieldKey fieldKey, ImmutableArray<FieldKey> queryProjectionKeys)
        {
            if (fieldKey is User.Contract.IUserFieldKeyCollection userFieldKeyCollection) return new ProjectLeadProjection(this.userServiceGetter)
            {
                UserKeys = UserFieldUtil.AddUserKeyFieldIfMissing(userFieldKeyCollection.Fields)
            };
            else return this;
        }

        public async Task<object> PrepareContext(IEnumerable<project> enties, CancellationToken cancellationToken = default)
        {
            var _enties = enties?.ToArray() ?? new project[0];

            if (_enties.Length == 0) return new Dictionary<string, IJiraUser>();

            var userKeys = _enties.Select(project => project.LEAD)
                .Distinct()
                .ToArray();

            var userService = userServiceGetter();
            var users = await userService.GetUsersByKeyAsync(userKeys, fields: UserKeys ?? userService.DefaultQueryFields.ToArray(), cancellationToken).ConfigureAwait(false);

            return users.ToDictionary(user => user.Key
                                    , user => user);
        }

        public Task Projection(project entity, JiraProject jiraProject, object context, CancellationToken cancellationToken = default)
        {
            var map = context as IDictionary<string, IJiraUser>;

            if (entity.LEAD != null && map.TryGetValue(entity.LEAD, out var leadUserInfo)) jiraProject.Lead = leadUserInfo;
            return Task.CompletedTask;
        }
    }

    internal class ProjectLeadSpecification : QuerySpecification<project>
    {
        public ProjectLeadSpecification(Expression<Func<string, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((project project) => project.LEAD, predicate));
        }
    }
}
