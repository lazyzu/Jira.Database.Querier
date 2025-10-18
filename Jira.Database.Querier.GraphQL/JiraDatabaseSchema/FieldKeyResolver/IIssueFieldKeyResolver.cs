using lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.GraphType.Issue;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.GraphQL.JiraDatabaseSchema.FieldKeyResolver
{
    public interface IIssueFieldKeyResolver
    {
        IEnumerable<Issue.Contract.FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields);
    }

    public class IssueFieldKeyResolver : IIssueFieldKeyResolver
    {
        private readonly IUserFieldKeyResolver userFieldKeyResolver;
        private readonly IProjectFieldKeyResolver projectFieldKeyResolver;

        private readonly Dictionary<string, FieldKey> customFieldKeyMap;

        public IssueFieldKeyResolver(IUserFieldKeyResolver userFieldKeyResolver
            , IProjectFieldKeyResolver projectFieldKeyResolver)
        {
            this.userFieldKeyResolver = userFieldKeyResolver;
            this.projectFieldKeyResolver = projectFieldKeyResolver;
            this.customFieldKeyMap = LoadCustomFieldKeyMap(IssueGraphType.CustomFieldSources);
        }

        public IEnumerable<FieldKey> Resolve(Dictionary<string, (GraphQLField Field, FieldType FieldType)> subFields)
        {
            foreach (var subField in subFields)
            {
                if (nameof(Issue.IJiraIssue.Project).ToCamelCase().Equals(subField.Key))
                {
                    var subFieldKeyOfProject = projectFieldKeyResolver.Resolve(subField.Value.Field).ToArray();
                    yield return IssueFieldSelection.ProjectWithField(subFieldKeyOfProject);
                }
                else if (nameof(Issue.IJiraIssue.Assignee).ToCamelCase().Equals(subField.Key))
                {
                    var subFiledKeyOfAssignee = userFieldKeyResolver.Resolve(subField.Value.Field).ToArray();
                    yield return IssueFieldSelection.AssigneeWithField(subFiledKeyOfAssignee);
                }
                else if (nameof(Issue.IJiraIssue.Reporter).ToCamelCase().Equals(subField.Key))
                {
                    var subFiledKeyOfReporter = userFieldKeyResolver.Resolve(subField.Value.Field).ToArray();
                    yield return IssueFieldSelection.ReporterWithField(subFiledKeyOfReporter);
                }
                else if (FieldKeyMap.TryGetValue(subField.Key, out var fieldKey)) yield return fieldKey;
                else if (customFieldKeyMap.TryGetValue(subField.Key, out var customFieldKey)) yield return customFieldKey;
            }
        }

        private static readonly Dictionary<string, Issue.Contract.FieldKey> FieldKeyMap
            = new Dictionary<string, Issue.Contract.FieldKey>
            {
                { nameof(Querier.Issue.IJiraIssue.Id).ToCamelCase(), IssueFieldSelection.IssueId  },
                { nameof(Querier.Issue.IJiraIssue.IssueNum).ToCamelCase(), IssueFieldSelection.IssueNum  },
                { nameof(Querier.Issue.IJiraIssue.Project).ToCamelCase(), IssueFieldSelection.Project  },
                { nameof(Querier.Issue.IJiraIssue.Key).ToCamelCase(), IssueFieldSelection.Key  },
                { nameof(Querier.Issue.IJiraIssue.Summary).ToCamelCase(), IssueFieldSelection.Summary  },
                { nameof(Querier.Issue.IJiraIssue.Description).ToCamelCase(), IssueFieldSelection.Description  },
                { nameof(Querier.Issue.IJiraIssue.CreateDate).ToCamelCase(), IssueFieldSelection.CreateDate  },
                { nameof(Querier.Issue.IJiraIssue.UpdateDate).ToCamelCase(), IssueFieldSelection.UpdateDate  },
                { nameof(Querier.Issue.IJiraIssue.DueDate).ToCamelCase(), IssueFieldSelection.DueDate  },
                { nameof(Querier.Issue.IJiraIssue.ResolutionDate).ToCamelCase(), IssueFieldSelection.ResolutionDate  },
                { nameof(Querier.Issue.IJiraIssue.SecurityLevel).ToCamelCase(), IssueFieldSelection.SecurityLevel  },
                { nameof(Querier.Issue.IJiraIssue.Assignee).ToCamelCase(), IssueFieldSelection.Assignee  },
                { nameof(Querier.Issue.IJiraIssue.Reporter).ToCamelCase(), IssueFieldSelection.Reporter  },
                { nameof(Querier.Issue.IJiraIssue.Environment).ToCamelCase(), IssueFieldSelection.Environment  },
                { nameof(Querier.Issue.IJiraIssue.Votes).ToCamelCase(), IssueFieldSelection.Votes  },
                { nameof(Querier.Issue.IJiraIssue.IssueStatus).ToCamelCase(), IssueFieldSelection.IssueStatus  },
                { nameof(Querier.Issue.IJiraIssue.Priority).ToCamelCase(), IssueFieldSelection.Priority  },
                { nameof(Querier.Issue.IJiraIssue.Resolution).ToCamelCase(), IssueFieldSelection.Resolution  },
                { nameof(Querier.Issue.IJiraIssue.IssueType).ToCamelCase(), IssueFieldSelection.IssueType  },
                { nameof(Querier.Issue.IJiraIssue.Components).ToCamelCase(), IssueFieldSelection.Components  },
                { nameof(Querier.Issue.IJiraIssue.AffectsVersions).ToCamelCase(), IssueFieldSelection.AffectsVersions  },
                { nameof(Querier.Issue.IJiraIssue.FixVersions).ToCamelCase(), IssueFieldSelection.FixVersions  },
                { nameof(Querier.Issue.IJiraIssue.Labels).ToCamelCase(), IssueFieldSelection.Labels  },
                { nameof(Querier.Issue.IJiraIssue.Comments).ToCamelCase(), IssueFieldSelection.Comments  },
                { nameof(Querier.Issue.IJiraIssue.Worklogs).ToCamelCase(), IssueFieldSelection.Worklogs  },
                { nameof(Querier.Issue.IJiraIssue.Changelogs).ToCamelCase(), IssueFieldSelection.Changelogs  },
                { nameof(Querier.Issue.IJiraIssue.ParentIssueId).ToCamelCase(), IssueFieldSelection.ParentIssueId  },
                { nameof(Querier.Issue.IJiraIssue.SubTaskIds).ToCamelCase(), IssueFieldSelection.SubTaskIds  },
                { nameof(Querier.Issue.IJiraIssue.IssueLinks).ToCamelCase(), IssueFieldSelection.IssueLinks  },
                { nameof(Querier.Issue.IJiraIssue.RemoteLinks).ToCamelCase(), IssueFieldSelection.RemoteLinks  },
                { nameof(Querier.Issue.IJiraIssue.Attachments).ToCamelCase(), IssueFieldSelection.Attachments  }
            };

        public static Dictionary<string, FieldKey> LoadCustomFieldKeyMap(IEnumerable<ICustomFieldSource> customFieldSources)
        {
            if (customFieldSources?.Any() ?? false)
            {
                return customFieldSources.SelectMany(source => source.GetEnumerable())
                    .ToDictionary(define => define.name.ToCamelCase()
                                , define => define.key as FieldKey);

            }
            else return new Dictionary<string, FieldKey>();
        }
    }
}
