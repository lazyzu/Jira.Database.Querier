using GraphQL;
using GraphQL.Types;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue.CustomField;
using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.Query;
using lazyzu.Jira.Database.Querier.Issue;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue
{
    public class IssueGraphType : ObjectGraphType<Querier.Issue.IJiraIssue>
    {
        public const string TypeName = "Issue";

        public IssueGraphType()
        {
            Name = TypeName;

            Field(i => i.Id).Description("");
            Field(i => i.IssueNum).Description("");
            Field(i => i.Project).Description("");
            Field(i => i.Key).Description("");
            Field(i => i.Summary).Description("");
            Field(i => i.Description).Description("");
            Field(i => i.CreateDate).Description("");
            Field(i => i.UpdateDate).Description("");
            Field(i => i.DueDate).Description("");
            Field(i => i.ResolutionDate).Description("");
            Field(i => i.SecurityLevel).Description("");
            Field(i => i.Assignee).Description("");
            Field(i => i.Reporter).Description("");
            Field(i => i.Environment).Description("");
            Field(i => i.Votes).Description("");
            // Field(i => i.CustomFields);
            Field(i => i.IssueStatus).Description("");
            Field(i => i.Priority).Description("");
            Field(i => i.Resolution).Description("");
            Field(i => i.IssueType).Description("");
            Field(i => i.Components).Description("");
            Field(i => i.AffectsVersions).Description("");
            Field(i => i.FixVersions).Description("");
            Field(i => i.Labels).Description("");
            Field(i => i.Comments).Description("");
            Field(i => i.Worklogs).Description("");
            Field(i => i.ParentIssueId).Description("");
            Field(i => i.SubTaskIds).Description("");
            Field(i => i.IssueLinks).Description("");
            Field(i => i.RemoteLinks).Description("");
            Field(i => i.Attachments).Description("");

            if (CustomFieldSources.Any())
            {
                CustomFieldHandler?.AddCustomFields(this, CustomFieldSources);
            }
        }

        public static IIssueGraphField_CustomFieldHandler CustomFieldHandler { get; set; }

        public static List<ICustomFieldSource> CustomFieldSources { get; private set; } = new List<ICustomFieldSource>();
    }

    public interface IIssueGraphField_CustomFieldHandler
    {
        void AddCustomFields(ObjectGraphType<Querier.Issue.IJiraIssue> jiraIssueGraphType, IEnumerable<ICustomFieldSource> customFieldSources);
    }

    public class IssueGraphField_CustomFieldHandler : IIssueGraphField_CustomFieldHandler
    {
        public void AddCustomFields(ObjectGraphType<IJiraIssue> jiraIssueGraphType, IEnumerable<ICustomFieldSource> customFieldSources)
        {
            foreach (var customFieldSource in customFieldSources)
            {
                foreach (var define in customFieldSource.GetEnumerable())
                {
                    if (define.key is CustomFieldKey<StringCustomFieldSchema> stringField)
                    {
                        Handle(jiraIssueGraphType, define.name, stringField, fieldValue => fieldValue.Value);   // <StringCustomFieldSchema, StringGraphType>
                    }
                    else if (define.key is CustomFieldKey<TextCustomFieldSchema> textField)
                    {
                        Handle(jiraIssueGraphType, define.name, textField, fieldValue => fieldValue.Value); // <TextCustomFieldSchema, StringGraphType>
                    }
                    else if (define.key is CustomFieldKey<NumberCustomFieldSchema> numberField)
                    {
                        Handle(jiraIssueGraphType, define.name, numberField, fieldValue => fieldValue.Value);   // <NumberCustomFieldSchema, DecimalGraphType>
                    }
                    else if (define.key is CustomFieldKey<DateTimeCustomFieldSchema> datetimeField)
                    {
                        Handle(jiraIssueGraphType, define.name, datetimeField, fieldValue => fieldValue.Value); // <DateTimeCustomFieldSchema, DateTimeGraphType>
                    }
                    else if (define.key is CustomFieldKey<LabelCustomFieldSchema> labelField)
                    {
                        Handle(jiraIssueGraphType, define.name, labelField, fieldValue => fieldValue.Value);
                    }
                    else if (define.key is CustomFieldKey<UserCustomFieldSchema> userField)
                    {
                        Handle(jiraIssueGraphType, define.name, userField, fieldValue => fieldValue.Value);
                    }
                    else if (define.key is CustomFieldKey<MultiUserCustomFieldSchema> multiUserField)
                    {
                        Handle(jiraIssueGraphType, define.name, multiUserField, fieldValue => fieldValue.Value);
                    }
                    else if (define.key is CustomFieldKey<SelectCustomFieldSchema> selectField)
                    {
                        Handle(jiraIssueGraphType, define.name, selectField, fieldValue => fieldValue.Value);
                    }
                    else if (define.key is CustomFieldKey<MultiSelectCustomFieldSchema> multiSelectField)
                    {
                        Handle(jiraIssueGraphType, define.name, multiSelectField, fieldValue => fieldValue.Value);
                    }
                    else if (define.key is CustomFieldKey<CascadingSelectCustomFieldSchema> cascadingSelectField)
                    {
                        Handle(jiraIssueGraphType, define.name, cascadingSelectField, fieldValue => fieldValue.Value);
                    }
                }
            }
        }

        public void Handle<TSchemaType, TOutputFieldType>(ObjectGraphType<IJiraIssue> jiraIssueGraphType, string name, CustomFieldKey<TSchemaType> customField, Func<TSchemaType, TOutputFieldType> converter, Action<FieldType> configure = null)
        {
            var builder = jiraIssueGraphType.Field<TOutputFieldType>(name.ToCamelCase(), nullable: true).Resolve(c =>
            {
                if (c.Source.CustomFields.TryGetValue(customField, out var vaule))
                {
                    return converter.Invoke(vaule);
                }
                else return default(TOutputFieldType);
            });

            if (configure != null) builder.Configure(configure);
        }
    }

    public interface ICustomFieldSource
    {
        IEnumerable<CustomFieldDefine> GetEnumerable();
    }

    public record CustomFieldDefine(string name, ICustomFieldKey key);

    public class IssueCustomFieldSchemaTypeMapping : IIssueCustomFieldSchemaTypeMapping
    {
        public void AddSchemaTypeMapping(JiraDatabaseSchema schema)
        {
            schema.RegisterTypeMapping<Querier.User.IJiraUser, User.UserGraphType>();
            schema.RegisterTypeMapping<Querier.Project.IJiraProject, Project.ProjectGraphType>();

            schema.RegisterTypeMapping<Querier.Issue.Fields.Custom.ISelectOption, SelectOptionGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.Custom.ISelectCollection, SelectCollectionGraphType>();
            schema.RegisterTypeMapping<Querier.Issue.Fields.Custom.ICascadingSelection, CascadingSelectGraphType>();
        }
    }
}
