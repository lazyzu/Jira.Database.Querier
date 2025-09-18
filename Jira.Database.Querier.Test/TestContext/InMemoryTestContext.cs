using Bogus;
using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.Fake;
using lazyzu.Jira.Database.Querier.Fake.DatabaseInitializer;
using lazyzu.Jira.Database.Querier.Issue;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Test.TestContext
{
    public class InMemoryTestContext
    {
        protected readonly string jiraServerUrl;

        protected readonly JiraDatabaseQuerierBuilder jiraDatabaseQuerierBuilder;
        protected readonly JiraContext jiraContext;

        internal protected readonly JiraUserFake jiraUserFaker;
        protected readonly DatabaseUserInitializer databaseUserInitializer;
        protected bool userContextInited = false;

        internal protected readonly JiraProjectFake jiraProjectFaker;
        protected readonly DatabaseProjectInitializer databaseProjectInitializer;
        protected bool projectContextInited = false;

        internal protected readonly JiraIssueFake jiraIssueFaker;
        protected readonly DatabaseIssueInitializer databaseIssueInitializer;
        protected bool issueContextInited = false;

        protected bool disposedValue;

        public InMemoryTestContext(string jiraServerUrl = Constants.JiraServerUrl
            , JiraUserFake.ConstructArgument fakeUserInitializerConstructArgument = null
            , JiraProjectFake.ConstructArgument fakeProjectInitializerConstructArgument = null
            , JiraIssueFake.ConstructArgument issueFakerConstructArgument = null)
        {
            this.jiraServerUrl = jiraServerUrl;

            jiraDatabaseQuerierBuilder = new JiraDatabaseQuerierBuilder(new Uri(jiraServerUrl));

            var dbContextOptionBuilder = new DbContextOptionsBuilder<JiraContext>();
            dbContextOptionBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            jiraContext = new JiraContext(dbContextOptionBuilder.Options);

            jiraUserFaker = new JiraUserFake(fakeUserInitializerConstructArgument ?? JiraUserFake.ConstructArgument.Default);
            databaseUserInitializer = new DatabaseUserInitializer();

            jiraProjectFaker = new JiraProjectFake(fakeProjectInitializerConstructArgument ?? JiraProjectFake.ConstructArgument.Default);
            databaseProjectInitializer = new DatabaseProjectInitializer();

            jiraIssueFaker = new JiraIssueFake(issueFakerConstructArgument ?? JiraIssueFake.ConstructArgument.Default);
            databaseIssueInitializer = new DatabaseIssueInitializer();
        }

        public const int randomSeeed = 8675309;

        static InMemoryTestContext()
        {
            Randomizer.Seed = new Random(randomSeeed);
        }

        public async Task InitUserContext()
        {
            if (userContextInited == false)
            {
                await databaseUserInitializer.GroupSelection.InitGroups(jiraUserFaker.ParentGroups, jiraUserFaker.CascadingChildGroups, jiraContext);
            }
        }

        public async Task<IJiraUser> GenerateUsers()
            => (await GenerateUsers(1)).First();

        public async Task<IJiraUser[]> GenerateUsers(int count)
        {
            var fakeUsers = jiraUserFaker.Generate(count);

            foreach (var fakeUser in fakeUsers) await databaseUserInitializer.AddUser(fakeUser, jiraContext, saveChange: false);
            jiraContext.SaveChanges();

            return fakeUsers;
        }

        public async Task InitProjectContext()
        {
            await InitUserContext();

            if (projectContextInited == false)
            {
                await databaseProjectInitializer.FieldSelection.AddIssueType(jiraProjectFaker.IssueTypeSelections, jiraContext, saveChange: false);
                await databaseProjectInitializer.FieldSelection.AddProjectRole(jiraProjectFaker.ProjectRoleSelections, jiraContext, saveChange: false);
                await databaseProjectInitializer.FieldSelection.AddProjectCategory(jiraProjectFaker.ProjectCategorySelections, jiraContext, saveChange: false);
                await databaseProjectInitializer.FieldSelection.AddSecurityLevelScheme(jiraProjectFaker.SecurityLevelSchemeSelections, jiraContext, saveChange: false);

                jiraContext.SaveChanges();
            }
        }

        public async Task<IJiraProject> GenerateProject(ProjectGenerateArgument projectGenerateArgument)
            => (await GenerateProjects(1, projectGenerateArgument)).First();

        public async Task<IJiraProject[]> GenerateProjects(int count, ProjectGenerateArgument projectGenerateArgument)
        {
            if (projectGenerateArgument.Groups == null) projectGenerateArgument.Groups = jiraUserFaker.Groups;

            var fakeProjects = jiraProjectFaker.Generate(count, projectGenerateArgument);
            foreach (var fakeProject in fakeProjects) await databaseProjectInitializer.AddJiraProject(fakeProject, jiraContext, saveChange: false);
            jiraContext.SaveChanges();

            return fakeProjects;
        }

        public class ProjectGenerateArgument : JiraProjectFake.GenerateArgument
        {
            public ProjectGenerateArgument(IEnumerable<IJiraUser> users) : base(users, null)
            { }
        }

        public async Task InitIssueContext()
        {
            await InitProjectContext();

            if (issueContextInited == false)
            {
                await databaseIssueInitializer.FieldSelection.AddIssueStatus(jiraIssueFaker.IssueStatusSelections, jiraContext, saveChange: false);
                await databaseIssueInitializer.FieldSelection.AddIssuePriority(jiraIssueFaker.IssuePrioritySelections, jiraContext, saveChange: false);
                await databaseIssueInitializer.FieldSelection.AddIssueResolution(jiraIssueFaker.IssueResolutionSelections, jiraContext, saveChange: false);
                await databaseIssueInitializer.FieldSelection.AddIssueLinkTypes(jiraIssueFaker.IssueLinkTypeSelections, jiraContext, saveChange: false);

                jiraContext.SaveChanges();
            }
        }

        public async Task<IJiraIssue> GenerateIssue(JiraIssueFake.GenerateArgument issueGenerateArgument)
            => (await GenerateIssue(1, issueGenerateArgument)).First();

        public async Task<IJiraIssue[]> GenerateIssue(int count, JiraIssueFake.GenerateArgument issueGenerateArgument)
        {
            var fakeIssues = jiraIssueFaker.Generate(count, issueGenerateArgument);

            foreach (var fakeIssue in fakeIssues) await databaseIssueInitializer.AddJiraIssue(fakeIssue, jiraContext, saveChange: false);
            jiraContext.SaveChanges();

            return fakeIssues;
        }

        public async Task<IJiraIssue> MoveIssue(IJiraIssue issue, IJiraProject project)
        {
            var originalKey = issue.Key;
            var movedIssue = jiraIssueFaker.Move(issue as JiraIssue, project);
            await databaseIssueInitializer.MoveIssue(movedIssue, originalKey, jiraContext, saveChange: true);

            return movedIssue;
        }

        public Task LinkIssue(IJiraIssue outwardIssue, IJiraIssue inwardIssue, IIssueLinkType issueLinkType)
            => databaseIssueInitializer.Link(outwardIssue, inwardIssue, issueLinkType, jiraContext, saveChange: true);

        public Task TestWithDatabase(Func<IJiraDatabaseQuerier, Task> test)
            => test(jiraDatabaseQuerierBuilder.Build(jiraContext, logger: null));

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    jiraContext.Dispose();
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        // ~InMemoryTestContext()
        // {
        //     // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
