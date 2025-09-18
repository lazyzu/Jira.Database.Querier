using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Test.TestContext;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Test
{
    internal class IssueServiceTest : IDisposable
    {
        private readonly InMemoryTestContext testContext;

        public IssueServiceTest()
        {
            this.testContext = new InMemoryTestContext();
            this.testContext.InitIssueContext().Wait();
        }

        [Test]
        public async Task IssueKeyService_GetIssueIdAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var goldenIssues = await testContext.GenerateIssue(count: 5, new Fake.JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));
                var goldenIssueKeys = goldenIssues.Select(issue => issue.Key).ToArray();

                var expected = goldenIssues.ToDictionary(issue => issue.Key, issue => issue.Id);
                var actual = await jiraDatabaseQuerier.Issue.IssueKey.GetIssueIdAsync(goldenIssueKeys);

                Assert.That(actual, Is.EquivalentTo(expected));
            });
        }

        [Test]
        public async Task IssueKeyService_GetIssueIdAsync_Moved()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                await testContext.GenerateIssue(count: 3, new Fake.JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));
                var goldenIssue = await testContext.GenerateIssue(new Fake.JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));
                var originalKey = goldenIssue.Key;

                var movedGoldenIssue = await testContext.MoveIssue(goldenIssue, referenceProjects.First(project => project.Id != goldenIssue.Project.Id));

                var actual = await jiraDatabaseQuerier.Issue.IssueKey.GetIssueIdAsync(originalKey);

                Assert.That(actual, Is.EqualTo(movedGoldenIssue.Id));
            });
        }

        [Test]
        public async Task IssuePriorityService_GetPrioritiesAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraIssueFaker.IssuePrioritySelections;
                var actual = await jiraDatabaseQuerier.Issue.IssuePriority.GetPrioritiesAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssuePriority>(actual, expected, priority => priority.Id);
            });
        }

        [Test]
        public async Task IssueResolutionService_GetResolutionsAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraIssueFaker.IssueResolutionSelections;
                var actual = await jiraDatabaseQuerier.Issue.IssueResolution.GetResolutionsAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueResolution>(actual, expected, resolution => resolution.Id);
            });
        }

        [Test]
        public async Task IssueSecurityLevelService_GetSecurityLevelSchemasAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraProjectFaker.SecurityLevelSchemeSelections.Select(x => x.Scheme).ToArray();
                var actual = await jiraDatabaseQuerier.Issue.IssueSecurityLevel.GetSecurityLevelSchemasAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueSecurityLevelScheme>(actual, expected, scheme => scheme.Id);
            });
        }

        [Test]
        public async Task IssueSecurityLevelService_GetSecurityLevelsAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraProjectFaker.SecurityLevelSchemeSelections.SelectMany(x => x.SecurityLevels).ToArray();
                var actual = await jiraDatabaseQuerier.Issue.IssueSecurityLevel.GetSecurityLevelsAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueSecurityLevel>(actual, expected, level => level.Id);
            });
        }

        [Test]
        public async Task IssueStatusService_GetStatusesAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraIssueFaker.IssueStatusSelections;
                var actual = await jiraDatabaseQuerier.Issue.IssueStatus.GetStatusesAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueStatus>(actual, expected, status => status.Id);
            });
        }

        [Test]
        public async Task IssueStatusService_GetStatusByIdAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var goldenStatusSelections = this.testContext.jiraIssueFaker.IssueStatusSelections;

                foreach (var expected in goldenStatusSelections)
                {
                    var actual = await jiraDatabaseQuerier.Issue.IssueStatus.GetStatusByIdAsync(expected.Id);
                    AssertUtil.MemberwisePropertiesEqual<IssueStatus>(actual, expected);
                }
            });
        }

        [Test]
        public async Task IssueStatusService_GetStatusByNameAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var goldenStatusSelections = this.testContext.jiraIssueFaker.IssueStatusSelections;

                foreach (var expected in goldenStatusSelections)
                {
                    var actual = await jiraDatabaseQuerier.Issue.IssueStatus.GetStatusByNameAsync(expected.Name);
                    AssertUtil.MemberwisePropertiesEqual<IssueStatus>(actual, expected);
                }
            });
        }

        [Test]
        public async Task IssueStatusCategoryService_GetStatusCategoryAsync_Args_Id()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var statusCategories = await jiraDatabaseQuerier.Issue.IssueStatusCategory.GetStatusCategoriesAsync();
                foreach (var expected in statusCategories)
                {
                    var actual = await jiraDatabaseQuerier.Issue.IssueStatusCategory.GetStatusCategoryAsync(expected.Id);
                    AssertUtil.MemberwisePropertiesEqual<IssueStatusCategory>(actual, expected);
                }
            });
        }

        [Test]
        public async Task IssueStatusCategoryService_GetStatusCategoryAsync_Args_Name()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var statusCategories = await jiraDatabaseQuerier.Issue.IssueStatusCategory.GetStatusCategoriesAsync();
                foreach (var expected in statusCategories)
                {
                    var actual = await jiraDatabaseQuerier.Issue.IssueStatusCategory.GetStatusCategoryAsync(expected.Name);
                    AssertUtil.MemberwisePropertiesEqual<IssueStatusCategory>(actual, expected);
                }
            });
        }

        [Test]
        public async Task IssueTypeService_GetIssueTypesAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraProjectFaker.IssueTypeSelections;
                var actual = await jiraDatabaseQuerier.Issue.IssueType.GetIssueTypesAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueType>(actual, expected, issueType => issueType.Id);
            });
        }

        [Test]
        public async Task IssueTypeService_GetIssueTypesForProjectAsync_Args_projectKey()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var goldenProjects = await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                foreach(var project in goldenProjects) 
                {
                    var expected = project.IssueTypeScheme.IssueTypes;
                    var actual = await jiraDatabaseQuerier.Issue.IssueType.GetIssueTypesForProjectAsync(project.Key).ToArrayAsync();

                    AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueType>(actual, expected, issueType => issueType.Id);
                }
            });
        }

        [Test]
        public async Task IssueTypeService_GetIssueTypesForProjectAsync_Args_projectId()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var goldenProjects = await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                foreach (var project in goldenProjects)
                {
                    var expected = project.IssueTypeScheme.IssueTypes;
                    var actual = await jiraDatabaseQuerier.Issue.IssueType.GetIssueTypesForProjectAsync(project.Id).ToArrayAsync();

                    AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueType>(actual, expected, issueType => issueType.Id);
                }
            });
        }

        [Test]
        public async Task IssueLinkService_GetLinkTypesAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraIssueFaker.IssueLinkTypeSelections;
                var actual = await jiraDatabaseQuerier.Issue.IssueLink.GetLinkTypesAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueLinkType>(actual, expected, issueLinkType => issueLinkType.Id);
            });
        }

        [Test]
        public async Task IssueLinkService_GetLinksForIssueAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var outwardIssue = await testContext.GenerateIssue(new Fake.JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));
                var inwardIssue = await testContext.GenerateIssue(new Fake.JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers));

                await testContext.LinkIssue(outwardIssue, inwardIssue, JiraDatabaseQuerierBuilder.DefaultSubtaskLinkType);

                var actualIssueLinks = await jiraDatabaseQuerier.Issue.IssueLink.GetLinksForIssueAsync(outwardIssue.Id);
                Assert.That(actualIssueLinks.Length, Is.EqualTo(1));

                var actualIssueLink = actualIssueLinks.First();
                Assert.That(actualIssueLink.LinkType, Is.EqualTo(JiraDatabaseQuerierBuilder.DefaultSubtaskLinkType));
                Assert.That(actualIssueLink.InwardIssueId, Is.EqualTo(inwardIssue.Id));
                Assert.That(actualIssueLink.OutwardIssueId, Is.EqualTo(outwardIssue.Id));
            });
        }

        public void Dispose()
        {
            testContext.Dispose();
        }
    }
}
