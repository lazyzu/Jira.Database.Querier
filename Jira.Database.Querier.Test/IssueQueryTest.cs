using lazyzu.Jira.Database.Querier.Fake;
using lazyzu.Jira.Database.Querier.Issue;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.Test.TestContext;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Test
{
    internal class IssueQueryTest : IDisposable
    {
        private readonly InMemoryTestContext testContext;

        public IssueQueryTest()
        {
            this.testContext = new InMemoryTestContext();
            this.testContext.InitIssueContext().Wait();
        }

        [Test]
        public async Task QueryIssueById_DefaultField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                });

                foreach (var goldenIssue in await testContext.GenerateIssue(count: 1, new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)))
                {
                    var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(goldenIssue.Id);
                    CheckProps(issueInfo, goldenIssue, jiraDatabaseQuerier.Issue.DefaultQueryFields, jiraDatabaseQuerier.Project.DefaultQueryFields, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        [Test]
        public async Task QueryIssue_ParentSubTask()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                });

                var parentIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));
                var subTaskIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));

                await testContext.LinkIssue(outwardIssue: parentIssue, inwardIssue: subTaskIssue, JiraDatabaseQuerierBuilder.DefaultSubtaskLinkType);

                // Check Sub-Task from parent
                var actualParentIssue = await jiraDatabaseQuerier.Issue.GetIssueAsync(parentIssue.Id, fields: new FieldKey[]
                {
                    IssueFieldSelection.SubTaskIds
                });
                Assert.That(actualParentIssue.SubTaskIds, Is.EquivalentTo(new decimal[] { subTaskIssue.Id }));

                // Check parent from Sub-Task
                var actualSubTaskIssue = await jiraDatabaseQuerier.Issue.GetIssueAsync(subTaskIssue.Id, fields: new FieldKey[]
                {
                    IssueFieldSelection.ParentIssueId
                });
                Assert.That(actualSubTaskIssue.ParentIssueId, Is.EqualTo(parentIssue.Id));
            });
        }

        [Test]
        public async Task QueryIssueByKey_DefaultField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                });

                foreach (var goldenIssue in await testContext.GenerateIssue(count: 1, new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)))
                {
                    var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(goldenIssue.Key);
                    CheckProps(issueInfo, goldenIssue, jiraDatabaseQuerier.Issue.DefaultQueryFields, jiraDatabaseQuerier.Project.DefaultQueryFields, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        [Test]
        public async Task QueryIssueByKey_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                });

                foreach (var goldenIssue in await testContext.GenerateIssue(count: 1, new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    ComponentCount = new RandomRange(1, 5),
                    AffectsVersionCount = new RandomRange(1, 5),
                    LabelCount = new RandomRange(1, 5),
                    CommentCount = new RandomRange(1, 5),
                    WorklogCount = new RandomRange(1, 5),
                    ChangelogCount = new RandomRange(1, 5),
                    RemoteLinkCount = new RandomRange(1, 5),
                    AttachmentCount = new RandomRange(1, 5)
                }))
                {
                    var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(goldenIssue.Key, IssueFieldSelection.AllNativeWithOption(new IssueFieldSelection.FieldOption
                    {
                        AssigneeFields = UserFieldSelection.All.ToArray(),
                        ReporterFields = UserFieldSelection.All.ToArray(),
                        ProjectFields = ProjectFieldSelection.AlleWithOption(new ProjectFieldSelection.FieldOption
                        {
                            ProjectLeadFields = UserFieldSelection.All.ToArray()
                        }).ToArray()
                    }).ToArray());
                    CheckProps(issueInfo, goldenIssue, IssueFieldSelection.AllNative, ProjectFieldSelection.All, UserFieldSelection.All);
                }
            });
        }

        // Search
        // var tutuAsignedIssues = await jiraDatabaseQuerier.Issue.SearchIssueAsync(specs =>
        // {
        //     return new QuerySpecification.IQuerySpecification[]
        //     {
        //                 specs.IssueProject(tsProject),
        //                 specs.IssueAssignee(tutuUserInfo)
        //     };
        // });

        [Test]
        public async Task QueryIssuesByKey_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                });

                var goldenIssues = await testContext.GenerateIssue(count: 1, new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));

                var issueKeys = goldenIssues.Select(issue => issue.Key).ToArray();
                var issueInfos = await jiraDatabaseQuerier.Issue.GetIssuesAsync(issueKeys, fields: IssueFieldSelection.AllNativeWithOption(new IssueFieldSelection.FieldOption
                {
                    AssigneeFields = UserFieldSelection.All.ToArray(),
                    ReporterFields = UserFieldSelection.All.ToArray(),
                    ProjectFields = ProjectFieldSelection.AlleWithOption(new ProjectFieldSelection.FieldOption
                    {
                        ProjectLeadFields = UserFieldSelection.All.ToArray()
                    }).ToArray()
                }).ToArray());

                foreach (var goldenIssue in goldenIssues)
                {
                    var matchedIssueInfo = issueInfos.FirstOrDefault(issue => issue.Id == goldenIssue.Id);

                    if (matchedIssueInfo == null) Assert.Fail("user is missing");
                    else CheckProps(matchedIssueInfo, goldenIssue, IssueFieldSelection.AllNative, ProjectFieldSelection.All, UserFieldSelection.All);
                }
            });

        }

        [Test]
        public async Task QueryIssuesById_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                });

                var goldenIssues = await testContext.GenerateIssue(count: 1, new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));

                var issueIds = goldenIssues.Select(issue => issue.Id).ToArray();
                var issueInfos = await jiraDatabaseQuerier.Issue.GetIssuesAsync(issueIds, fields: IssueFieldSelection.AllNativeWithOption(new IssueFieldSelection.FieldOption
                {
                    AssigneeFields = UserFieldSelection.All.ToArray(),
                    ReporterFields = UserFieldSelection.All.ToArray(),
                    ProjectFields = ProjectFieldSelection.AlleWithOption(new ProjectFieldSelection.FieldOption
                    {
                        ProjectLeadFields = UserFieldSelection.All.ToArray()
                    }).ToArray()
                }).ToArray());

                foreach (var goldenIssue in goldenIssues)
                {
                    var matchedIssueInfo = issueInfos.FirstOrDefault(issue => issue.Id == goldenIssue.Id);

                    if (matchedIssueInfo == null) Assert.Fail("user is missing");
                    else CheckProps(matchedIssueInfo, goldenIssue, IssueFieldSelection.AllNative, ProjectFieldSelection.All, UserFieldSelection.All);
                }
            });
        }

        public static void CheckProps(IJiraIssue actual, IJiraIssue expected
            , IEnumerable<Issue.Contract.FieldKey> checkFields
            , IEnumerable<Project.Contract.FieldKey> checkProjectFields
            , IEnumerable<User.Contract.FieldKey> checkUserFields)
        {
            if (checkFields == null) Assert.Fail("No any check fields");
            else
            {
                var _checkFields = checkFields.ToArray();
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                if (_checkFields.Contains(IssueFieldSelection.IssueNum)) Assert.That(actual.IssueNum, Is.EqualTo(expected.IssueNum));
                if (_checkFields.Contains(IssueFieldSelection.Project)) ProjectQueryTest.CheckProps(actual.Project, expected.Project, checkProjectFields, checkUserFields);
                if (_checkFields.Contains(IssueFieldSelection.Key)) Assert.That(actual.Key, Is.EqualTo(expected.Key));
                if (_checkFields.Contains(IssueFieldSelection.Summary)) Assert.That(actual.Summary, Is.EqualTo(expected.Summary));
                if (_checkFields.Contains(IssueFieldSelection.Description)) Assert.That(actual.Description, Is.EqualTo(expected.Description));
                if (_checkFields.Contains(IssueFieldSelection.CreateDate)) Assert.That(actual.CreateDate, Is.EqualTo(expected.CreateDate));
                if (_checkFields.Contains(IssueFieldSelection.UpdateDate)) Assert.That(actual.UpdateDate, Is.EqualTo(expected.UpdateDate));
                if (_checkFields.Contains(IssueFieldSelection.DueDate)) Assert.That(actual.DueDate, Is.EqualTo(expected.DueDate));
                if (_checkFields.Contains(IssueFieldSelection.ResolutionDate)) Assert.That(actual.ResolutionDate, Is.EqualTo(expected.ResolutionDate));
                if (_checkFields.Contains(IssueFieldSelection.SecurityLevel)) AssertUtil.MemberwisePropertiesEqual<IssueSecurityLevel>(actual.SecurityLevel, expected.SecurityLevel);
                if (_checkFields.Contains(IssueFieldSelection.Assignee)) UserQueryTest.CheckProps(actual.Assignee, expected.Assignee, checkUserFields);
                if (_checkFields.Contains(IssueFieldSelection.Reporter)) UserQueryTest.CheckProps(actual.Reporter, expected.Reporter, checkUserFields);
                if (_checkFields.Contains(IssueFieldSelection.Environment)) Assert.That(actual.Environment, Is.EqualTo(expected.Environment));
                if (_checkFields.Contains(IssueFieldSelection.Votes)) Assert.That(actual.Votes, Is.EqualTo(expected.Votes));
                if (_checkFields.Contains(IssueFieldSelection.IssueStatus)) AssertUtil.MemberwisePropertiesEqual<IssueStatus>(actual.IssueStatus, expected.IssueStatus);
                if (_checkFields.Contains(IssueFieldSelection.Priority)) AssertUtil.MemberwisePropertiesEqual<IssuePriority>(actual.Priority, expected.Priority);
                if (_checkFields.Contains(IssueFieldSelection.Resolution)) AssertUtil.MemberwisePropertiesEqual<IssueResolution>(actual.Resolution, expected.Resolution);
                if (_checkFields.Contains(IssueFieldSelection.IssueType)) AssertUtil.MemberwisePropertiesEqual<IssueType>(actual.IssueType, expected.IssueType);
                if (_checkFields.Contains(IssueFieldSelection.Components)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<ProjectComponent>(actual.Components, expected.Components, component => component.Id);
                if (_checkFields.Contains(IssueFieldSelection.AffectsVersions)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<ProjectVersion>(actual.AffectsVersions, expected.AffectsVersions, version => version.Id);
                if (_checkFields.Contains(IssueFieldSelection.FixVersions)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<ProjectVersion>(actual.FixVersions, expected.FixVersions, version => version.Id);
                if (_checkFields.Contains(IssueFieldSelection.Labels)) Assert.That(actual.Labels, Is.EquivalentTo(expected.Labels));
                if (_checkFields.Contains(IssueFieldSelection.Comments)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueComment>(actual.Comments, expected.Comments, comment => comment.Id);
                if (_checkFields.Contains(IssueFieldSelection.Worklogs)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueWorklog>(actual.Worklogs, expected.Worklogs, worklog => worklog.Id);
                if (_checkFields.Contains(IssueFieldSelection.Changelogs)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueChangelog>(actual.Changelogs, expected.Changelogs, changeLog => changeLog.Id, (actual, expected) =>
                {
                    Assert.That(actual.Author, Is.EqualTo(expected.Author));
                    Assert.That(actual.Created, Is.EqualTo(expected.Created));
                    Assert.That(actual.Id, Is.EqualTo(expected.Id));

                    foreach (var expectedChangeItem in expected.Items)
                    {
                        var actualChangeItem = actual.Items.FirstOrDefault(item => expectedChangeItem.Field.Equals(item.Field));
                        if (actualChangeItem == null) Assert.Fail("Missing change item");
                        else AssertUtil.MemberwisePropertiesEqual<IssueChangelogItem>(actualChangeItem, expectedChangeItem);
                    }
                });
                if (_checkFields.Contains(IssueFieldSelection.ParentIssueId)) Assert.That(actual.ParentIssueId, Is.EqualTo(expected.ParentIssueId));
                //if (_checkFields.Contains(IssueFieldSelection.SubTaskIds)) Assert.That(actual.SubTaskIds, Is.EquivalentTo(expected.SubTaskIds));
                //if (_checkFields.Contains(IssueFieldSelection.IssueLinks)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueLink>(actual.IssueLinks, expected.IssueLinks, issueLink => issueLink.Id);
                if (_checkFields.Contains(IssueFieldSelection.RemoteLinks)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueRemoteLink>(actual.RemoteLinks, expected.RemoteLinks, remoteLink => remoteLink.Id);
                if (_checkFields.Contains(IssueFieldSelection.Attachments)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueAttachment>(actual.Attachments, expected.Attachments, attachment => attachment.Id);
            }
        }

        public void Dispose()
        {
            testContext.Dispose();
        }
    }
}
