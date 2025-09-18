using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.EntityFrameworkCore.Model;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.QuerySpecification;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Issue.Fields
{
    public class IssueAffectsVersionsProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }
        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public IssueAffectsVersionsProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.AffectsVersions
            };

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.AffectsVersions
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueAffectVersionMap = await ProjectVersionExtension.LoadIssueVersionMap(issueIds, "IssueVersion", jiraContext, cancellationToken).ConfigureAwait(false);

                if (issueAffectVersionMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueAffectVersionMap.TryGetValue(issue.Id, out var affectVersions)) issue.AffectsVersions = affectVersions;
                        else issue.AffectsVersions = new IProjectVersion[0];
                    }
                }
                else foreach (var issue in _issues)
                {
                    issue.AffectsVersions = new IProjectVersion[0];
                }
            }
        }
    }

    internal class IssueAffectsVersionsSpecification : QuerySpecification<nodeassociation>
    {
        public IssueAffectsVersionsSpecification(Expression<Func<decimal?, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((nodeassociation nodeassociation) => nodeassociation.SINK_NODE_ID, predicate
                                                             , (nodeassociation nodeassociation) => nodeassociation.SINK_NODE_ENTITY == "Version"
                                                                                                 && nodeassociation.ASSOCIATION_TYPE == "IssueVersion");
        }
    }
}
