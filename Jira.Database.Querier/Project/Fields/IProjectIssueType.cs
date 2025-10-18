using Generator.Equals;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Issue.Services;
using lazyzu.Jira.Database.Querier.Project.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Project.Fields
{
    public interface IIssueTypeScheme
    {
        decimal Id { get; }
        IIssueType[] IssueTypes { get; }
    }

    [Equatable(Explicit = true)]
    public partial class IssueTypeScheme : IIssueTypeScheme
    {
        [DefaultEquality]
        public decimal Id { get; init; }
        public IIssueType[] IssueTypes { get; init; }
    }

    public class ProjectIssueTypeProjection : IProjectExternalProjectionSpecification
    {
        public IEnumerable<FieldKey> HandleTarget { get; protected init; }

        protected readonly JiraContext jiraContext;
        protected readonly Func<IIssueTypeService> issueTypeServiceGetter;
        protected readonly SharedCache cache;
        protected readonly ILogger logger;

        public ProjectIssueTypeProjection(JiraContext jiraContext, Func<IIssueTypeService> issueTypeServiceGetter, SharedCache cache, ILogger logger)
        {
            this.jiraContext = jiraContext;
            this.issueTypeServiceGetter = issueTypeServiceGetter;
            this.cache = cache;
            this.logger = logger;

            HandleTarget = new FieldKey[]
            {
                ProjectFieldSelection.ProjectIssueType
            };
        }

        public ProjectIssueTypeProjection(JiraContext jiraContext, Func<IJiraDatabaseQuerier> jiraDatabaseQuerierGetter, SharedCache cache, ILogger logger)
            : this(jiraContext, () => jiraDatabaseQuerierGetter().Issue.IssueType, cache, logger)
        { }

        public virtual async Task Projection(IEnumerable<JiraProject> projects, CancellationToken cancellationToken = default)
        {
            var _projects = projects?.ToArray() ?? new JiraProject[0];

            if (_projects.Any())
            {
                var projectIds = _projects.Select<JiraProject, decimal?>(project => project.Id).ToArray();

                var projectIssueTypes = await LoadProjectIssueTypes(projectIds, cancellationToken).ConfigureAwait(false);

                foreach (var project in _projects)
                {
                    if (projectIssueTypes.TryGetValue(project.Id, out var issueTypes)) project.IssueTypeScheme = issueTypes;
                    else project.IssueTypeScheme = null;
                }
            }
        }

        protected virtual async Task<Dictionary<decimal, IIssueTypeScheme>> LoadProjectIssueTypes(decimal?[] projectIds, CancellationToken cancellationToken)
        {
            var projectissueTypeSchemaQuery = from nodeassociation in jiraContext.nodeassociation.AsNoTracking()
                                              where projectIds.Contains(nodeassociation.SOURCE_NODE_ID)
                                              && nodeassociation.SINK_NODE_ENTITY == "IssueTypeScreenScheme"
                                              select new
                                              {
                                                  nodeassociation.SOURCE_NODE_ID,
                                                  nodeassociation.SINK_NODE_ID
                                              };

            var projectissueTypeSchemaQueryResult = await projectissueTypeSchemaQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var issueTypeSchemaIds = projectissueTypeSchemaQueryResult.Select(dbModel => dbModel.SINK_NODE_ID as decimal?).ToArray();

            var schemaIssueTypes = await LoadSchemaIssueTypes(issueTypeSchemaIds, cancellationToken).ConfigureAwait(false);

            return projectissueTypeSchemaQueryResult.ToDictionary(dbModel => dbModel.SOURCE_NODE_ID, dbModel =>
            {
                if (schemaIssueTypes.TryGetValue(dbModel.SINK_NODE_ID, out var issueTypes)) return new IssueTypeScheme
                {
                    Id = dbModel.SINK_NODE_ID,
                    IssueTypes = issueTypes
                } as IIssueTypeScheme;
                else return null;
            });
        }

        protected virtual async Task<Dictionary<decimal, IIssueType[]>> LoadSchemaIssueTypes(decimal?[] issueTypeSchemaIds, CancellationToken cancellationToken)
        {
            var result = new Dictionary<decimal, IIssueType[]>();
            if (issueTypeSchemaIds.Any() == false) return result;

            var issueTypeOfSchemaQuery = from issuetypescreenschemeentity in jiraContext.issuetypescreenschemeentity.AsNoTracking()
                                         where issueTypeSchemaIds.Contains(issuetypescreenschemeentity.SCHEME)
                                         select new
                                         {
                                             issuetypescreenschemeentity.SCHEME,
                                             issuetypescreenschemeentity.ISSUETYPE
                                         };
            
            var issueTypeOfSchemaQueryResult = await issueTypeOfSchemaQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);
            var issueTypeMap = await LoadIssueTypeMap(cancellationToken).ConfigureAwait(false);

            return issueTypeOfSchemaQueryResult.GroupBy(schemaIssueType => schemaIssueType.SCHEME)
                .ToDictionary(schemaIdGroup => schemaIdGroup.Key.Value
                            , schemaIdGroup =>
                            {
                                var issueTypeIds = schemaIdGroup.Select(schemaIssueType => schemaIssueType.ISSUETYPE);
                                return GetIssueTypes(issueTypeIds, issueTypeMap).ToArray();
                            });
        }

        protected virtual async Task<IDictionary<string, IIssueType>> LoadIssueTypeMap(CancellationToken cancellationToken)
        {
            IDictionary<string, IIssueType> issueTypes = cache.IssueTypes;
            if (issueTypes.Any() == false)
            {
                var issueTypeService = issueTypeServiceGetter();
                issueTypes = (await issueTypeService.GetIssueTypesAsync(cancellationToken).ConfigureAwait(false))
                    .ToDictionary(issueType => issueType.Id);
            }
            return issueTypes;
        }

        protected virtual IEnumerable<IIssueType> GetIssueTypes(IEnumerable<string> issueTypeIds, IDictionary<string, IIssueType> dict)
        {
            if (issueTypeIds != null)
            {
                foreach (var issueTypeId in issueTypeIds)
                {
                    if (issueTypeId != null && dict.TryGetValue(issueTypeId, out var issueType)) yield return issueType;
                }
            }
        }
    }
}
