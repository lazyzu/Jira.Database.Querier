using lazyzu.Jira.Database.Querier.Issue;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;
using lazyzu.Jira.Database.Querier.Issue.Fields.QuerySpecificationHandler;
using lazyzu.Jira.Database.Querier.Issue.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier
{
    public partial class JiraDatabaseQuerierBuilder
	{
		protected BuildServiceDelegate<IIssueKeyService> IssueKeyServiceBuilder;
		protected BuildServiceDelegate<IIssuePriorityService> IssuePriorityServiceBuilder;
		protected BuildServiceDelegate<IIssueResolutionService> IssueResolutionServiceBuilder;
		protected BuildServiceDelegate<IIssueSecurityLevelService> IssueSecurityLevelServiceBuilder;
		protected BuildServiceDelegate<IIssueStatusService> IssueStatusServiceBuilder;
        protected BuildServiceDelegate<IIssueStatusCategoryService> IssueStatusCategoryServiceBuilder;
        protected BuildServiceDelegate<IIssueTypeService> IssueTypeServiceBuilder;
		protected BuildServiceDelegate<IIssueLinkService> IssueLinkServiceBuilder;
		protected BuildServiceDelegate<IIssueCustomFieldService> IssueCustomFieldServiceBuilder;

        protected Func<Uri, ILogger, IIssueAttachmentUrlBuilder> IssueAttachmentUrlBuilder;
		protected BuildSpecificationDelegate<IIssueProjectionSpecification> IssueSimpleFieldProjectionBuilder;
		protected BuildSpecificationDelegate<IIssueCustomFieldProjectionSpecification> IssueCustomFieldProjectionBuilder;
		protected BuildSpecificationDelegate<IIssueQuerySpecificationHandler> IssueQuerySpecificationHandlerBuilder;
		protected Issue.Contract.FieldKey[] DefaultIssueQueryFields;
		protected IIssueLinkType SubtaskLinkType = DefaultSubtaskLinkType;

		public static readonly IIssueLinkType DefaultSubtaskLinkType = new IssueLinkType
        {
            Id = 10100,
            Name = "jira_subtask_link",
            Inward = "jira_subtask_inward",
            Outward = "jira_subtask_outward"
        };

        internal class IssueFieldServiceCollection : IIssueFieldServiceCollection
		{
			public IIssueKeyService IssueKey { get; init; }
			public IIssuePriorityService IssuePriority { get; init; }
			public IIssueResolutionService IssueResolution { get; init; }
			public IIssueSecurityLevelService IssueSecurityLevel { get; init; }
			public IIssueStatusService IssueStatus { get; init; }
			public IIssueStatusCategoryService IssueStatusCategory { get; init; }
            public IIssueTypeService IssueType { get; init; }
			public IIssueLinkService IssueLink { get; init; }
            public IIssueCustomFieldService IssueCustomField { get; init; }
        }

		public void UseIssueKeyService(BuildServiceDelegate<IIssueKeyService> build)
			=> IssueKeyServiceBuilder = build;

		public void UseIssuePriorityService(BuildServiceDelegate<IIssuePriorityService> build)
			=> IssuePriorityServiceBuilder = build;

		public void UseIssueResolutionService(BuildServiceDelegate<IIssueResolutionService> build)
			=> IssueResolutionServiceBuilder = build;

		public void UseIssueSecurityLevelService(BuildServiceDelegate<IIssueSecurityLevelService> build)
			=> IssueSecurityLevelServiceBuilder = build;

		public void UseIssueStatusService(BuildServiceDelegate<IIssueStatusService> build)
			=> IssueStatusServiceBuilder = build;

		public void UseIssueStatusCategoryService(BuildServiceDelegate<IIssueStatusCategoryService> build)
			=> IssueStatusCategoryServiceBuilder = build;


        public void UseIssueTypeService(BuildServiceDelegate<IIssueTypeService> build)
			=> IssueTypeServiceBuilder = build;

		public void UseIssueLinkService(BuildServiceDelegate<IIssueLinkService> build)
			=> IssueLinkServiceBuilder = build;

		public void UseIssueCustomFieldService(BuildServiceDelegate<IIssueCustomFieldService> build)
			=> IssueCustomFieldServiceBuilder = build;

        public void UseIssueAttachmentUrlBuilder(Func<Uri, ILogger, IIssueAttachmentUrlBuilder> build)
			=> IssueAttachmentUrlBuilder = build;

		public void UseIssueSimpleFieldProjection(DefaultIssueSimpleFieldProjectionOption defaultIssueSimpleFieldProjectionOption
			, BuildSpecificationDelegate<IIssueProjectionSpecification> buildExternal = null)
		{
			this.IssueSimpleFieldProjectionBuilder = (ServiceBuildContext context)
				=> useSimpleFieldProjection(context
				, defaultIssueSimpleFieldProjectionOption
                , buildExternal);
		}

		protected virtual IEnumerable<IIssueProjectionSpecification> useSimpleFieldProjection(ServiceBuildContext context
			, DefaultIssueSimpleFieldProjectionOption defaultIssueSimpleFieldProjectionOption
            , BuildSpecificationDelegate<IIssueProjectionSpecification> buildExternal)
		{
			if (defaultIssueSimpleFieldProjectionOption?.Enable ?? false)
			{
				yield return new IssueNumProjection();
				yield return new IssueProjectProjection(context.JiraDatabaseQuerierGetter);
				yield return new IssueKeyProjection(context.JiraDatabaseQuerierGetter);
				yield return new IssueSummaryProjection();
				yield return new IssueDescriptionProjection();
				yield return new IssueCreateDateProjection();
				yield return new IssueUpdateDateProjection();
				yield return new IssueDueDateProjection();
				yield return new IssueResolutionDateProjection();
				yield return new IssueSecurityLevelProjection(context.JiraDatabaseQuerierGetter, context.Cache);
				yield return new IssueAssigneeProjection(context.JiraDatabaseQuerierGetter);
				yield return new IssueReporterProjection(context.JiraDatabaseQuerierGetter);
				yield return new IssueEnvironmentProjection();
				yield return new IssueVotesProjection();
				yield return new IssueStatusProjection(context.JiraDatabaseQuerierGetter, context.Cache);
				yield return new IssuePriorityProjection(context.JiraDatabaseQuerierGetter, context.Cache);
				yield return new IssueResolutionProjection(context.JiraDatabaseQuerierGetter, context.Cache);
				yield return new IssueTypeProjection(context.JiraDatabaseQuerierGetter, context.Cache);
				yield return new IssueComponentProjection(context.JiraContext, context.Logger);
				yield return new IssueAffectsVersionsProjection(context.JiraContext, context.Logger);
				yield return new IssueFixVersionsProjection(context.JiraContext, context.Logger);
				yield return new IssueLabelProjection(context.JiraContext, context.Logger);
				yield return new IssueCommentProjection(context.JiraContext, context.Logger);
				yield return new IssueWorklogProjection(context.JiraContext, context.Logger);
				yield return new IssueChangelogProjection(context.JiraContext, context.Logger);
				yield return new IssueParentProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, jiraSubtaskLinkType: defaultIssueSimpleFieldProjectionOption.SubtaskLinkType, context.Logger);
				yield return new IssueSubTaskProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, jiraSubtaskLinkType: defaultIssueSimpleFieldProjectionOption.SubtaskLinkType, context.Logger);
				yield return new IssueLinkProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Logger);
				yield return new IssueRemoteLinkProjection(context.JiraContext, new IRemoteLinkResolver[]
				{
					new ConfluenceLinkResolver()
				}, context.Logger);
				yield return new IssueAttachmentProjection(context.JiraContext, context.IssueAttachmentUrlBuilder, context.Logger);
			}

			if (buildExternal != null)
			{
				foreach (var externalProjectProjection in buildExternal(context))
				{
					yield return externalProjectProjection;
				}
			}
		}

		public class DefaultIssueSimpleFieldProjectionOption
        {
			public bool Enable { get; init; }
			public IIssueLinkType SubtaskLinkType { get; init; }
        }

		public void UseIssueCustomFieldProjection(bool withDefault = true, BuildSpecificationDelegate<IIssueCustomFieldProjectionSpecification> buildExternal = null)
		{
			this.IssueCustomFieldProjectionBuilder = (ServiceBuildContext context)
				=> useCustomFieldProjection(context
				, withDefault
				, buildExternal);
		}

		protected virtual IEnumerable<IIssueCustomFieldProjectionSpecification> useCustomFieldProjection(ServiceBuildContext context
			, bool withDefault
			, BuildSpecificationDelegate<IIssueCustomFieldProjectionSpecification> buildExternal)
		{
			if (withDefault)
			{
				yield return new CascadingSelectCustomFieldProjection(context.JiraContext, context.Logger);
				yield return new DateTimeCustomFieldProjection(context.JiraContext, context.Logger);
				yield return new LabelCustomFieldProjection(context.JiraContext, context.Logger);
				yield return new MultiSelectCustomFieldProjection(context.JiraContext, context.Logger);
				yield return new MultiUserCustomFieldProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Logger);
				yield return new NumberCustomFieldProjection(context.JiraContext, context.Logger);
				yield return new SelectCustomFieldProjection(context.JiraContext, context.Logger);
				yield return new StringCustomFieldProjection(context.JiraContext, context.Logger);
				yield return new TextCustomFieldProjection(context.JiraContext, context.Logger);
				yield return new UserCustomFieldProjection(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Logger);
			}

			if (buildExternal != null)
			{
				foreach (var externalProjectProjection in buildExternal(context))
				{
					yield return externalProjectProjection;
				}
			}
		}

		public void UseIssueQuerySpecificationHandler(bool withDefault = true, BuildSpecificationDelegate<IIssueQuerySpecificationHandler> buildExternal = null)
		{
			this.IssueQuerySpecificationHandlerBuilder = (ServiceBuildContext context)
				=> useIssueQuerySpecificationHandler(context
				, withDefault
				, buildExternal);
		}

		protected virtual IEnumerable<IIssueQuerySpecificationHandler> useIssueQuerySpecificationHandler(ServiceBuildContext context
			, bool withDefault
			, BuildSpecificationDelegate<IIssueQuerySpecificationHandler> buildExternal)
		{
			if (withDefault)
			{
				yield return new JiraIssueQuerySpecificationHandler();
				yield return new LabelQuerySpecificationHandler();
				yield return new NodeAssociationQuerySpecificationHandler();
				yield return new CustomFieldValueQuerySpecificationHandler();
			}

			if (buildExternal != null)
			{
				foreach (var externalProjectProjection in buildExternal(context))
				{
					yield return externalProjectProjection;
				}
			}
		}

		public void UseDefaultIssueQueryField(Func<IEnumerable<Issue.Contract.FieldKey>> build)
		{
			this.DefaultIssueQueryFields = build().ToArray();
		}

		internal IIssueFieldServiceCollection BuildIssueFieldServiceCollection(ServiceBuildContext context)
		{
			var issueKeyService = IssueKeyServiceBuilder?.Invoke(context) 
				?? new IssueKeyService(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Cache, context.Logger);

			var issuePriorityService = IssuePriorityServiceBuilder?.Invoke(context) 
				?? new IssuePriorityService(context.JiraContext, context.Cache, context.Logger);

			var issueResolutionService = IssueResolutionServiceBuilder?.Invoke(context)
				?? new IssueResolutionService(context.JiraContext, context.Cache, context.Logger);

			var issueSecurityLevelService = IssueSecurityLevelServiceBuilder?.Invoke(context)
				?? new IssueSecurityLevelService(context.JiraContext, context.Cache, context.Logger);

			var issueStatusService = IssueStatusServiceBuilder?.Invoke(context)
				?? new IssueStatusService(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Cache, context.Logger);

			var issueStatusCategoryService = IssueStatusCategoryServiceBuilder?.Invoke(context)
				?? new IssueStatusCategoryService(context.JiraContext, context.Cache, context.Logger);


            var issueTypeService = IssueTypeServiceBuilder?.Invoke(context)
				?? new IssueTypeService(context.JiraContext, context.JiraDatabaseQuerierGetter, context.Cache, context.Logger);

			var issueLinkService = IssueLinkServiceBuilder?.Invoke(context)
				?? new IssueLinkService(context.JiraContext, context.Cache, context.Logger);

			var issueCustomFieldService = IssueCustomFieldServiceBuilder?.Invoke(context)
				?? new IssueCustomFieldService(context.JiraContext, context.Cache, context.Logger);

            return new IssueFieldServiceCollection
			{
				IssueKey = issueKeyService,
				IssuePriority = issuePriorityService,
				IssueResolution = issueResolutionService,
				IssueSecurityLevel = issueSecurityLevelService,
				IssueStatus = issueStatusService,
				IssueStatusCategory = issueStatusCategoryService,
                IssueType = issueTypeService,
				IssueLink = issueLinkService,
				IssueCustomField = issueCustomFieldService,
			};
		}

		internal IIssueService BuildIssueService(ServiceBuildContext context, IIssueFieldServiceCollection issueFieldServiceCollection)
		{
			var fieldProjectionSpecifications = IssueSimpleFieldProjectionBuilder?.Invoke(context) 
				?? useSimpleFieldProjection(context, new DefaultIssueSimpleFieldProjectionOption
				{
					Enable = true,
					SubtaskLinkType = SubtaskLinkType
				}, buildExternal: null);

			var customFieldProjectionSpecifications = IssueCustomFieldProjectionBuilder?.Invoke(context)
				?? useCustomFieldProjection(context, withDefault: true, buildExternal: null);

			var querySpecificationHandlers = IssueQuerySpecificationHandlerBuilder?.Invoke(context)
				?? useIssueQuerySpecificationHandler(context, withDefault: true, buildExternal: null);

			return new IssueService(context.JiraContext
				, fieldProjectionSpecifications.ToArray()
                , customFieldProjectionSpecifications.ToArray()
                , querySpecificationHandlers.ToArray()
                , DefaultIssueQueryFields ?? new FieldKey[]
				{
					IssueFieldSelection.Key,
                    IssueFieldSelection.Summary
                }
				, new IssueSpecs(context.JiraDatabaseQuerierGetter)
				, context.Logger)
			{
				IssueKey = issueFieldServiceCollection.IssueKey,
				IssuePriority = issueFieldServiceCollection.IssuePriority,
				IssueResolution = issueFieldServiceCollection.IssueResolution,
				IssueSecurityLevel = issueFieldServiceCollection.IssueSecurityLevel,
				IssueStatus = issueFieldServiceCollection.IssueStatus,
				IssueStatusCategory = issueFieldServiceCollection.IssueStatusCategory,
				IssueType = issueFieldServiceCollection.IssueType,
				IssueLink = issueFieldServiceCollection.IssueLink,
				IssueCustomField = issueFieldServiceCollection.IssueCustomField
            };
		}
    }
}
