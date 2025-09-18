using lazyzu.Jira.Database.Querier.Fake;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.Test.TestContext;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Test
{
    internal class ProjectQueryTest : IDisposable
    {
        private readonly InMemoryTestContext testContext;

        public ProjectQueryTest()
        {
            this.testContext = new InMemoryTestContext();
            this.testContext.InitProjectContext().Wait();
        }

        [Test]
        public async Task QueryProjectById_DefaultField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);

                foreach (var goldenProject in await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                }))
                {
                    var projectInfo = await jiraDatabaseQuerier.Project.GetProjectAsync(goldenProject.Id);
                    CheckProps(projectInfo, goldenProject, jiraDatabaseQuerier.Project.DefaultQueryFields, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        [Test]
        public async Task QueryProjectByKey_DefaultField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);

                foreach (var goldenProject in await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                }))
                {
                    var projectInfo = await jiraDatabaseQuerier.Project.GetProjectAsync(goldenProject.Key);
                    CheckProps(projectInfo, goldenProject, jiraDatabaseQuerier.Project.DefaultQueryFields, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        [Test]
        public async Task QueryProjectByKey_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);

                foreach (var goldenProject in await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                }))
                {
                    var projectInfo = await jiraDatabaseQuerier.Project.GetProjectAsync(goldenProject.Key, fields: ProjectFieldSelection.All.ToArray());
                    CheckProps(projectInfo, goldenProject, ProjectFieldSelection.All, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        [Test]
        public async Task QueryProjectsById_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);

                var goldenProjects = await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                });
                var projectIds = goldenProjects.Select(project => project.Id).ToArray();
                var projectInfos = await jiraDatabaseQuerier.Project.GetProjectsAsync(projectIds, fields: ProjectFieldSelection.All.ToArray());

                foreach (var goldenProject in goldenProjects)
                {
                    var matchedProjectInfo = projectInfos.FirstOrDefault(project => project.Id == goldenProject.Id);

                    if (matchedProjectInfo == null) Assert.Fail("user is missing");
                    else CheckProps(matchedProjectInfo, goldenProject, ProjectFieldSelection.All, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        [Test]
        public async Task QueryProjectsByKey_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);

                var goldenProjects = await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)
                {
                    ProjectComponentCount = new RandomRange(1, 5),
                    ProjectVersionCount = new RandomRange(1, 5)
                });
                var projectKeys = goldenProjects.Select(project => project.Key).ToArray();
                var projectInfos = await jiraDatabaseQuerier.Project.GetProjectsAsync(projectKeys, fields: ProjectFieldSelection.All.ToArray());

                foreach (var goldenProject in goldenProjects)
                {
                    var matchedProjectInfo = projectInfos.FirstOrDefault(project => project.Id == goldenProject.Id);

                    if (matchedProjectInfo == null) Assert.Fail("user is missing");
                    else CheckProps(matchedProjectInfo, goldenProject, ProjectFieldSelection.All, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        public static void CheckProps(IJiraProject actual, IJiraProject expected
            , IEnumerable<Project.Contract.FieldKey> checkFields
            , IEnumerable<User.Contract.FieldKey> checkUserFields)
        {
            if (checkFields == null) Assert.Fail("No any check fields");
            else
            {
                var _checkFields = checkFields.ToArray();
                Assert.That(actual.Id, Is.EqualTo(expected.Id));
                if (_checkFields.Contains(ProjectFieldSelection.ProjectName)) Assert.That(actual.Name, Is.EqualTo(expected.Name));
                if (_checkFields.Contains(ProjectFieldSelection.ProjectUrl)) Assert.That(actual.Url, Is.EqualTo(expected.Url));
                if (_checkFields.Contains(ProjectFieldSelection.ProjectLead)) UserQueryTest.CheckProps(actual.Lead, expected.Lead, checkUserFields);
                if (_checkFields.Contains(ProjectFieldSelection.ProjectDescription)) Assert.That(actual.Description, Is.EqualTo(expected.Description));
                if (_checkFields.Contains(ProjectFieldSelection.ProjectKey)) Assert.That(actual.Key, Is.EqualTo(expected.Key));
                if (_checkFields.Contains(ProjectFieldSelection.ProjectAvatar)) AssertUtil.MemberwisePropertiesEqual<ProjectAvatar>(actual.Avatar, expected.Avatar);
                if (_checkFields.Contains(ProjectFieldSelection.ProjectType)) Assert.That(actual.Type, Is.EqualTo(expected.Type));
                if (_checkFields.Contains(ProjectFieldSelection.ProjectCategory)) AssertUtil.MemberwisePropertiesEqual<ProjectCategory>(actual.Category, expected.Category);
                if (_checkFields.Contains(ProjectFieldSelection.ProjectRole)) AssertUtil.EquivalentToAndDefaultEqual<ProjectRoleActorMap>(actual.ProjectRoles, expected.ProjectRoles, projectRoleActorMap => projectRoleActorMap.Id);
                if (_checkFields.Contains(ProjectFieldSelection.ProjectIssueType))
                {
                    Assert.That(actual.IssueTypeScheme, Is.EqualTo(expected.IssueTypeScheme));
                    AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueType>(actual.IssueTypeScheme.IssueTypes, expected.IssueTypeScheme.IssueTypes, issueType => issueType.Id);
                }
                if (_checkFields.Contains(ProjectFieldSelection.ProjectComponent)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<ProjectComponent>(actual.Components, expected.Components, component => component.Id);
                if (_checkFields.Contains(ProjectFieldSelection.ProjectVersion)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<ProjectVersion>(actual.Versions, expected.Versions, version => version.Id);
                if (_checkFields.Contains(ProjectFieldSelection.ProjectIssueSecurityLevel)) AssertUtil.EquivalentToAndMemberwisePropertiesEqual<IssueSecurityLevel>(actual.SecurityLevels, expected.SecurityLevels, securityLevel => securityLevel.Id);
            }
        }

        public void Dispose()
        {
            testContext.Dispose();
        }
    }
}
