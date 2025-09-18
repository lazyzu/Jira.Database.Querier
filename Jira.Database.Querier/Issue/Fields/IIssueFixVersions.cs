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
    public class IssueFixVersionsProjection : IIssueExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly ILogger logger;

        public IssueFixVersionsProjection(JiraContext jiraContext, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                IssueFieldSelection.FixVersions
            };
        }

        public virtual async Task Projection(IEnumerable<JiraIssue> issues, CancellationToken cancellationToken = default)
        {
            var _issues = issues?.ToArray() ?? new JiraIssue[0];

            if (_issues.Any())
            {
                var issueIds = _issues.Select<JiraIssue, decimal?>(issue => issue.Id).ToArray();

                var issueFixVersionMap = await ProjectVersionExtension.LoadIssueVersionMap(issueIds, "IssueFixVersion", jiraContext, cancellationToken).ConfigureAwait(false);

                if (issueFixVersionMap.Any())
                {
                    foreach (var issue in _issues)
                    {
                        if (issueFixVersionMap.TryGetValue(issue.Id, out var fixVersions)) issue.FixVersions = fixVersions;
                        else issue.FixVersions = new IProjectVersion[0];
                    }
                }
                else foreach (var issue in _issues)
                {
                    issue.FixVersions = new IProjectVersion[0];
                }
            }
        }
    }

    internal class IssueFixVersionsSpecification : QuerySpecification<nodeassociation>
    {
        public IssueFixVersionsSpecification(Expression<Func<decimal?, bool>> predicate)
        {
            var criteria = QuerySpecificationExtension.Predict((nodeassociation nodeassociation) => nodeassociation.SINK_NODE_ID, predicate
                                                             , (nodeassociation nodeassociation) => nodeassociation.SINK_NODE_ENTITY == "Version"
                                                                                                 && nodeassociation.ASSOCIATION_TYPE == "IssueFixVersion");
        }
    }
}
