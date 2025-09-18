using Bogus;
using lazyzu.Jira.Database.Querier.Issue;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.User;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.Fake
{
    public class JiraIssueFake
    {
        public ImmutableArray<IIssueStatusCategory> IssueStatusCategorySelections { get; protected set; }
        public ImmutableArray<IIssueStatus> IssueStatusSelections { get; protected set; }
        public ImmutableArray<IIssuePriority> IssuePrioritySelections { get; protected set; }
        public ImmutableArray<IIssueResolution> IssueResolutionSelections { get; protected set; }
        public ImmutableArray<IIssueLinkType> IssueLinkTypeSelections { get; protected set; }

        public readonly FieldSelectionGenerator FieldSelection = new FieldSelectionGenerator();
        protected readonly FieldFake Field;

        protected long issueIdIdex = 0;
        protected readonly Dictionary<IJiraProject, int> projectIssueNumMap = new Dictionary<IJiraProject, int>();
        protected readonly string jiraServerUrl;

        public JiraIssueFake(ConstructArgument args = null)
        {
            var _args = args ?? ConstructArgument.Default;

            jiraServerUrl = _args.JiraServerUrl;

            IssueStatusCategorySelections = _args.IssueStatusCategorySelections.IsEmpty ? FieldSelection.GenerateIssueStatusCategories().ToImmutableArray() : _args.IssueStatusCategorySelections;
            IssueStatusSelections = _args.IssueStatusSelections.IsEmpty ? FieldSelection.GenerateIssueStatuses(IssueStatusCategorySelections).ToImmutableArray() : _args.IssueStatusSelections;
            IssuePrioritySelections = _args.IssuePrioritySelections.IsEmpty ? FieldSelection.GenerateIssuePriorities().ToImmutableArray() : _args.IssuePrioritySelections;
            IssueResolutionSelections = _args.IssueResolutionSelections.IsEmpty ? FieldSelection.GenerateIssueResolutions().ToImmutableArray() : _args.IssueResolutionSelections;
            IssueLinkTypeSelections = _args.IssueLinkTypeSelections.IsEmpty ? FieldSelection.GenerateIIssueLinkTypes().ToImmutableArray() : _args.IssueLinkTypeSelections;

            Field = new FieldFake(jiraServerUrl);
        }

        public IJiraIssue[] Generate(int count, GenerateArgument args)
        {
            var boolSelections = new[] { true, false };

            var issueFake = new Faker<JiraIssue>()
                //.StrictMode(true)
                .RuleFor(issue => issue.Id, faker => issueIdIdex++)
                .Rules((faker, issue) =>
                {
                    var project = faker.PickRandom(args.ProjectSelections);

                    // Issue Project
                    issue.Project = project;

                    // Issue Num
                    if (projectIssueNumMap.ContainsKey(issue.Project)) issue.IssueNum = ++projectIssueNumMap[issue.Project];
                    else
                    {
                        var startIssueNum = 1;
                        projectIssueNumMap.Add(issue.Project, startIssueNum);
                        issue.IssueNum = startIssueNum;
                    }

                    // Security Levels
                    issue.SecurityLevel = faker.PickRandom(project.SecurityLevels);

                    // Components
                    issue.Components = faker.PickRandom(project.Components, amountToPick: faker.Random.Int(args.ComponentCount.Min, Math.Min(args.ComponentCount.Max, project.Components.Length))).ToArray();

                    // IssueType
                    issue.IssueType = faker.PickRandom(project.IssueTypeScheme.IssueTypes);

                    // AffectsVersions, FixVersions
                    var hasFixVersions = faker.PickRandom(boolSelections);
                    if (project.Versions.Any() && hasFixVersions) issue.FixVersions = new Project.Fields.IProjectVersion[]
                    {
                        faker.PickRandom(project.Versions)
                    }; 
                    else issue.FixVersions = new Project.Fields.IProjectVersion[0];

                    var hasAffectsVersions = faker.PickRandom(boolSelections);
                    if (hasAffectsVersions) issue.AffectsVersions = new Project.Fields.IProjectVersion[0];
                    else issue.AffectsVersions = faker.PickRandom(project.Versions.AsEnumerable(), amountToPick: faker.Random.Int(args.AffectsVersionCount.Min, Math.Min(args.AffectsVersionCount.Max, project.Versions.Length)))
                        .Except(issue.FixVersions)
                        .ToArray();
                })
                .RuleFor(issue => issue.Summary, (faker, issue) => $"{issue.Id} Summary")
                .RuleFor(issue => issue.Description, (faker, issue) => $"{issue.Id} Description")
                .RuleFor(issue => issue.DueDate, faker => faker.Date.Future())
                .RuleFor(issue => issue.UpdateDate, faker => faker.Date.Past())
                .RuleFor(issue => issue.CreateDate, (faker, issue) => faker.Date.Past(refDate: issue.UpdateDate))
                .RuleFor(issue => issue.Resolution, faker =>
                {
                    var isClosed = faker.PickRandom(boolSelections);
                    if (isClosed) return faker.PickRandom(IssueResolutionSelections.AsEnumerable());
                    else return null;
                })
                .RuleFor(issue => issue.ResolutionDate, (faker, issue) =>
                {
                    if (issue.Resolution == null) return null;
                    else return issue.UpdateDate;
                })
                .RuleFor(issue => issue.Assignee, faker => faker.PickRandom(args.UserSelections))
                .RuleFor(issue => issue.Reporter, faker => faker.PickRandom(args.UserSelections))
                .RuleFor(issue => issue.Environment, (faker, issue) => $"{issue.Id} Environment")
                .RuleFor(issue => issue.Votes, faker => faker.Random.UInt())
                .RuleFor(issue => issue.IssueStatus, faker => faker.PickRandom(IssueStatusSelections.AsEnumerable()))
                .RuleFor(issue => issue.Priority, faker => faker.PickRandom(IssuePrioritySelections.AsEnumerable()))
                .RuleFor(issue => issue.Labels, faker => faker.Random.WordsArray(args.LabelCount.Min, args.LabelCount.Max))
                .RuleFor(issue => issue.Comments, faker => Field.GenerateIssueComment(faker.Random.Int(args.CommentCount.Min, args.CommentCount.Max), authorSelections: args.UserSelections).ToArray())
                .RuleFor(issue => issue.Worklogs, faker => Field.GenerateIssueWorklog(faker.Random.Int(args.WorklogCount.Min, args.WorklogCount.Max), authorSelections: args.UserSelections).ToArray())
                .RuleFor(issue => issue.ParentIssueId, faker => null)
                .RuleFor(issue => issue.SubTaskIds, faker => new decimal[0])
                // IssueLinks
                .RuleFor(issue => issue.Changelogs, (faker, issue) => Field.GenerateIssueChangelog(faker.Random.Int(args.ChangelogCount.Min, args.ChangelogCount.Max), issue, authorSelections: args.UserSelections).ToArray())
                .RuleFor(issue => issue.RemoteLinks, faker => Field.GenerateIssueRemoteLink(faker.Random.Int(args.RemoteLinkCount.Min, args.RemoteLinkCount.Max)).ToArray())
                .RuleFor(issue => issue.Attachments, faker => Field.GenerateIssueAttachment(faker.Random.Int(args.AttachmentCount.Min, args.AttachmentCount.Max), authorSelections: args.UserSelections).ToArray())
                .RuleFor(issue => issue.CustomFields, (faker, issue) =>
                {
                    var customFields = new CustomFieldValueCollection();
                    args.RenderCustomField?.Invoke(customFields, issue, faker);
                    return customFields;
                } );

            // CustomFieldValueCollection

            return issueFake.Generate(count).ToArray();
        }

        public IJiraIssue Move(JiraIssue issue, IJiraProject project)
        {
            // Issue Project
            issue.Project = project;

            // Issue Num
            if (projectIssueNumMap.ContainsKey(issue.Project)) issue.IssueNum = ++projectIssueNumMap[issue.Project];
            else
            {
                var startIssueNum = 1;
                projectIssueNumMap.Add(issue.Project, startIssueNum);
                issue.IssueNum = startIssueNum;
            }

            return issue;
        }


        public class FieldSelectionGenerator
        {
            public IEnumerable<IIssueStatusCategory> GenerateIssueStatusCategories()
            {
                var defaultNames = new string[]
                {
                "No Category", "To Do", "In Progress", "Done"
                };

                var id = 1;

                foreach (var name in defaultNames) yield return new IssueStatusCategory()
                {
                    Id = id++,
                    Name = name
                };
            }

            public IEnumerable<IIssueStatus> GenerateIssueStatuses(IEnumerable<IIssueStatusCategory> statusCategories)
            {
                foreach (var statusCategory in statusCategories)
                {
                    yield return new IssueStatus()
                    {
                        Id = (statusCategory.Id).ToString(),
                        Name = statusCategory.Name,
                        Description = $"{statusCategory.Name} Description",
                        Category = statusCategory
                    };
                }
            }

            public IEnumerable<IIssuePriority> GenerateIssuePriorities()
            {
                var defaultNames = new string[]
                {
                "Highest", "High", "Medium", "Low", "Lowest"
                };

                var id = 0;

                foreach (var name in defaultNames) yield return new IssuePriority()
                {
                    Id = (id++).ToString(),
                    Name = name,
                    Description = $"{name} Description",
                };
            }

            public IEnumerable<IIssueResolution> GenerateIssueResolutions()
            {
                var id = 0;

                const string FIXED = "FIXED";
                yield return new IssueResolution
                {
                    Id = (id++).ToString(),
                    Name = FIXED,
                    Description = $"{FIXED} Description"
                };

                const string DUPLICATE = "DUPLICATE";
                yield return new IssueResolution
                {
                    Id = (id++).ToString(),
                    Name = DUPLICATE,
                    Description = $"{DUPLICATE} Description"
                };

                const string WONTFIX = "WONTFIX";
                yield return new IssueResolution
                {
                    Id = (id++).ToString(),
                    Name = WONTFIX,
                    Description = $"{WONTFIX} Description"
                };

                const string Done = "Done";
                yield return new IssueResolution
                {
                    Id = (id++).ToString(),
                    Name = Done,
                    Description = $"{Done} Description"
                };
            }

            public IEnumerable<IIssueLinkType> GenerateIIssueLinkTypes()
            {
                yield return new IssueLinkType
                {
                    Id = 10000,
                    Name = "Blocks",
                    Inward = "is blocked by",
                    Outward = "blocks",
                };

                yield return new IssueLinkType
                {
                    Id = 10001,
                    Name = "Cloners",
                    Inward = "is cloned by",
                    Outward = "clones",
                };

                yield return new IssueLinkType
                {
                    Id = 10002,
                    Name = "Duplicate",
                    Inward = "is duplicated by",
                    Outward = "duplicates",
                };

                yield return new IssueLinkType
                {
                    Id = 10003,
                    Name = "Relates",
                    Inward = "relates to",
                    Outward = "relates to",
                };

                yield return new IssueLinkType
                {
                    Id = 10100,
                    Name = "jira_subtask_link",
                    Inward = "jira_subtask_inward",
                    Outward = "jira_subtask_outward",
                };
            }
        }

        public class FieldFake
        {
            protected readonly string jiraServerUrl;

            protected long jiraactionIdIndex = 0;
            protected long issueWorklogIdIndex = 0;
            protected long issueChangelogIdIndex = 0;
            protected long issueAttachmentIdIndex = 0;

            protected readonly Faker<IssueRemoteLink> issueRemoteLinkFake;

            public FieldFake(string jiraServerUrl)
            {
                this.jiraServerUrl = jiraServerUrl;
                this.issueRemoteLinkFake = BuildRemoteLinkFake();
            }

            public virtual IEnumerable<IIssueComment> GenerateIssueComment(int count, IEnumerable<IJiraUser> authorSelections)
            {
                var _authorSelections = authorSelections.ToArray();

                var commentFake = new Faker<IssueComment>()
                    .StrictMode(true)
                    .RuleFor(comment => comment.Id, faker => jiraactionIdIndex++)
                    .RuleFor(comment => comment.Author, faker => faker.PickRandom(_authorSelections).Key)   // TODO: NEED TO CHECK
                    .RuleFor(comment => comment.UpdateAuthor, (faker, comment) => comment.Author)
                    .RuleFor(comment => comment.Updated, faker => faker.Date.Past())
                    .RuleFor(comment => comment.Created, (faker, comment) => faker.Date.Past(refDate: comment.Updated))
                    .RuleFor(comment => comment.Body, faker => faker.Lorem.Paragraph());

                return commentFake.Generate(count);
            }

            public virtual IEnumerable<IIssueWorklog> GenerateIssueWorklog(int count, IEnumerable<IJiraUser> authorSelections)
            {
                var _authorSelections = authorSelections.ToArray();

                var worklogFake = new Faker<IssueWorklog>()
                    .StrictMode(true)
                    .RuleFor(worklog => worklog.Id, faker => issueWorklogIdIndex++)
                    .RuleFor(worklog => worklog.Author, faker => faker.PickRandom(_authorSelections).Key)   // TODO: NEED TO CHECK
                    .RuleFor(worklog => worklog.UpdateAuthor, (faker, worklog) => worklog.Author)
                    .RuleFor(worklog => worklog.Updated, faker => faker.Date.Past())
                    .RuleFor(worklog => worklog.Created, (faker, worklog) => faker.Date.Past(refDate: worklog.Updated))
                    .RuleFor(worklog => worklog.Started, (faker, worklog) => faker.Date.Past(refDate: worklog.Updated))
                    .RuleFor(worklog => worklog.TimeSpent, faker => faker.Date.Timespan())
                    .RuleFor(worklog => worklog.Comment, faker => faker.Lorem.Paragraph());

                return worklogFake.Generate(count);
            }

            public virtual IEnumerable<IIssueChangelog> GenerateIssueChangelog(int count, IJiraIssue issue, IEnumerable<IJiraUser> authorSelections)
            {
                var _authorSelections = authorSelections.ToArray();

                var issueChangelogFake = new Faker<IssueChangelog>()
                    .StrictMode(true)
                    .RuleFor(issueChangelog => issueChangelog.Id, faker => issueChangelogIdIndex++)
                    .RuleFor(issueChangelog => issueChangelog.Author, faker => faker.PickRandom(_authorSelections).Key)
                    .RuleFor(issueChangelog => issueChangelog.Created, faker => issue.UpdateDate)
                    .RuleFor(issueChangelog => issueChangelog.Items, faker =>
                    {
                        return new IIssueChangelogItem[]
                        {
                        new IssueChangelogItem()
                        {
                            Field = "status",
                            FieldType = "jira",
                            OldValue = "old status value",
                            OldString = "old status string",
                            NewValue = issue.IssueStatus.Id,
                            NewString = issue.IssueStatus.Name
                        },
                        new IssueChangelogItem()
                        {
                            Field = "custom",
                            FieldType = "custom",
                            OldValue = "old custom filed value",
                            OldString = "old custom filed string",
                            NewValue = "new custom filed value",
                            NewString = "new custom filed string"
                        }
                        };
                    });

                return issueChangelogFake.Generate(count);

            }

            public virtual IEnumerable<IIssueRemoteLink> GenerateIssueRemoteLink(int count)
                => issueRemoteLinkFake.Generate(count);

            protected virtual Faker<IssueRemoteLink> BuildRemoteLinkFake()
            {
                return new Faker<IssueRemoteLink>()
                    .StrictMode(true)
                    .RuleFor(link => link.Id, faker => faker.IndexFaker)
                    .RuleFor(link => link.RemoteUrl, faker => faker.Internet.Url())
                    .RuleFor(link => link.Title, faker => faker.Lorem.Word())
                    .RuleFor(link => link.Summary, faker => faker.Lorem.Word())
                    .RuleFor(link => link.Relationship, faker => faker.Lorem.Word());
            }

            protected Dictionary<string, string> mimeTypeExtensionMap = new Dictionary<string, string>
            {
                { "application/json", ".json" },
                { "image/jpeg", ".jpeg" },
                { "application/pdf", ".pdf" },
                { "text/plain", ".txt" },
                { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx" },
                { "application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx" },
                { "application/zip", ".zip" },
            };

            public virtual IEnumerable<IIssueAttachment> GenerateIssueAttachment(int count, IEnumerable<IJiraUser> authorSelections)
            {
                var _authorSelections = authorSelections.ToArray();

                var attachmentFake = new Faker<IssueAttachment>()
                .StrictMode(true)
                .RuleFor(attachment => attachment.Id, faker => issueAttachmentIdIndex++)
                .RuleFor(attachment => attachment.MimeType, faker => faker.PickRandom(mimeTypeExtensionMap.AsEnumerable()).Key)
                .RuleFor(attachment => attachment.FileName, (faker, attachment) => $"{faker.Lorem.Word()}{mimeTypeExtensionMap[attachment.MimeType]}")
                .RuleFor(attachment => attachment.Author, faker => faker.PickRandom(_authorSelections).Key)
                .RuleFor(attachment => attachment.Created, faker => faker.Date.Past())
                .RuleFor(attachment => attachment.Size, faker => faker.Random.UInt())
                .RuleFor(attachment => attachment.Content, (faker, attachment) =>
                {
                    return new Uri(new Uri(jiraServerUrl), relativeUri: $@"secure/attachment/{attachment.Id}/{attachment.FileName}");
                });

                return attachmentFake.Generate(count);
            }
        }

        public class ConstructArgument
        {
            public string JiraServerUrl { get; set; } = Constants.JiraServerUrl;

            public ImmutableArray<IIssueStatusCategory> IssueStatusCategorySelections { get; set; } = ImmutableArray<IIssueStatusCategory>.Empty;
            public ImmutableArray<IIssueStatus> IssueStatusSelections { get; set; } = ImmutableArray<IIssueStatus>.Empty;
            public ImmutableArray<IIssuePriority> IssuePrioritySelections { get; set; } = ImmutableArray<IIssuePriority>.Empty;
            public ImmutableArray<IIssueResolution> IssueResolutionSelections { get; set; } = ImmutableArray<IIssueResolution>.Empty;
            public ImmutableArray<IIssueLinkType> IssueLinkTypeSelections { get; set; } = ImmutableArray<IIssueLinkType>.Empty;

            public static readonly ConstructArgument Default = new ConstructArgument();
        }

        public class GenerateArgument
        {
            public readonly IJiraProject[] ProjectSelections;
            public readonly IJiraUser[] UserSelections;

            public RandomRange ComponentCount { get; set; } = new RandomRange(0, 5);
            public RandomRange AffectsVersionCount { get; set; } = new RandomRange(0, 5);

            public RandomRange LabelCount { get; set; } = new RandomRange(0, 5);
            public RandomRange CommentCount { get; set; } = new RandomRange(0, 5);
            public RandomRange WorklogCount { get; set; } = new RandomRange(0, 5);
            public RandomRange ChangelogCount { get; set; } = new RandomRange(0, 5);
            public RandomRange RemoteLinkCount { get; set; } = new RandomRange(0, 5);
            public RandomRange AttachmentCount { get; set; } = new RandomRange(0, 5);

            public delegate void RenderCustomFieldDelegate(CustomFieldValueCollection customFields, IJiraIssue issue, Bogus.Faker faker);
            public RenderCustomFieldDelegate RenderCustomField;

            public GenerateArgument(IEnumerable<IJiraProject> projectSelections
            , IEnumerable<IJiraUser> userSelections)
            {
                this.ProjectSelections = projectSelections?.ToArray() ?? new IJiraProject[0];
                this.UserSelections = userSelections?.ToArray() ?? new IJiraUser[0];
            }
        }
    }
}
