using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public interface IIssueSecurityLevel
    {
        decimal Id { get; }
        string Name { get; }
        string Description { get; }
        IIssueSecurityLevelScheme Scheme { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueSecurityLevel : IIssueSecurityLevel
    {
        [DefaultEquality]
        public decimal Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public IIssueSecurityLevelScheme Scheme { get; init; }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Description})";
        }
    }

    public interface IIssueSecurityLevelScheme
    {
        decimal Id { get; }
        string Name { get; }
        string Description { get; }
        IIssueSecurityLevel DefaultValue { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueSecurityLevelScheme : IIssueSecurityLevelScheme
    {
        [DefaultEquality]
        public decimal Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public IIssueSecurityLevel DefaultValue { get; init; }

        public IssueSecurityLevelScheme()
        { }

        public IssueSecurityLevelScheme(decimal id, string name, string description, (decimal id, string name, string description)? defaultValue)
        {
            Id = id;
            Name = name;
            Description = description;

            if (defaultValue.HasValue)
            {
                DefaultValue = new IssueSecurityLevel
                {
                    Id = defaultValue.Value.id,
                    Name = defaultValue.Value.name,
                    Description = defaultValue.Value.description,
                    Scheme = this
                };
            }
        }

        public override string ToString()
        {
            return $"{Id}:{Name} ({Description}, DefaultValue: {DefaultValue?.Name ?? "null"})";
        }
    }

    public class IssueSecurityLevelProjection : IIssueProjectionSpecification<jiraissue>
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        public Expression<Func<jiraissue, object>>[] IncludeExpressions { get; protected init; }

        private readonly Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter;
        private readonly SharedCache cache;

        public IssueSecurityLevelProjection(Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache)
        {
            this.jiraDatabaseQuerierGetter = jiraDatabaseQuerierGetter;
            this.cache = cache;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.SecurityLevel
            };

            IncludeExpressions = new Expression<Func<jiraissue, object>>[]
            {
                issue => issue.SECURITY
            };
        }

        public virtual async Task Projection(jiraissue entity, JiraIssue jiraIssue, CancellationToken cancellationToken = default)
        {
            IDictionary<decimal, IIssueSecurityLevel> securityLevels = cache.SecurityLevels;
            if (securityLevels.Any() == false)
            {
                var securityLevelService = jiraDatabaseQuerierGetter().Issue.IssueSecurityLevel;
                securityLevels = (await securityLevelService.GetSecurityLevelsAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
                    .ToDictionary(securityLevel => securityLevel.Id);
            }

            if (entity.SECURITY.HasValue && securityLevels.TryGetValue(entity.SECURITY.Value, out var securityLevel)) jiraIssue.SecurityLevel = securityLevel;
        }
    }

    public class IssueSecurityLevelIdSpecification : QuerySpecification<jiraissue>
    {
        public IssueSecurityLevelIdSpecification(Expression<Func<decimal?, bool>> predicate)
        {
            CriteriaGetter = () => Task.FromResult(QuerySpecificationExtension.Predict((jiraissue issue) => issue.SECURITY, predicate));
        }
    }
}
