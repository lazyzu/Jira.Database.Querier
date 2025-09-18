using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.Test.TestContext;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Test
{
    internal class ProjectServiceTest : IDisposable
    {
        private readonly InMemoryTestContext testContext;

        public ProjectServiceTest()
        {
            this.testContext = new InMemoryTestContext();
            this.testContext.InitProjectContext().Wait();
        }

        [Test]
        public async Task ProjectCategoryService_GetProjectCategoriesAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraProjectFaker.ProjectCategorySelections;
                var actual = await jiraDatabaseQuerier.Project.ProjectCategory.GetProjectCategoriesAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<ProjectCategory>(actual, expected, projectCategory => projectCategory.Id);
            });
        }

        [Test]
        public async Task ProjectKeyService_GetProjectIdAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);

                foreach (var goldenProject in await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)))
                {
                    var expected = goldenProject.Id;
                    var actual = await jiraDatabaseQuerier.Project.ProjectKey.GetProjectIdAsync(goldenProject.Key);
                    Assert.That(actual, Is.EqualTo(expected));
                }
            });
        }

        [Test]
        public async Task ProjectKeyService_GetProjectIdsAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);

                var goldenProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));
                var expected = goldenProjects.ToDictionary(project => project.Key, project => project.Id);

                var goldenProjectKeys = goldenProjects.Select(project => project.Key).ToArray();
                var actual = await jiraDatabaseQuerier.Project.ProjectKey.GetProjectIdsAsync(goldenProjectKeys);

                Assert.That(actual, Is.EquivalentTo(expected));
            });
        }

        [Test]
        public async Task ProjectRoleService_GetProjectRolesAsync()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var expected = this.testContext.jiraProjectFaker.ProjectRoleSelections;
                var actual = await jiraDatabaseQuerier.Project.ProjectRole.GetProjectRolesAsync();

                AssertUtil.EquivalentToAndMemberwisePropertiesEqual<ProjectRole>(actual, expected, projectRole => projectRole.Id);
            });
        }

        [Test]
        public async Task ProjectRoleService_GetProjectRolesAsync_Args_ProjectId()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);

                foreach (var goldenProject in await testContext.GenerateProjects(1, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers)))
                {
                    var expected = goldenProject.ProjectRoles.Cast<IProjectRole>().ToArray();
                    var actual = await jiraDatabaseQuerier.Project.ProjectRole.GetProjectRolesAsync(goldenProject.Id);

                    AssertUtil.EquivalentToAndMemberwisePropertiesEqual<ProjectRole>(actual, expected, projectRole => projectRole.Id);
                }
            });
        }

        public void Dispose()
        {
            testContext.Dispose();
        }
    }
}
