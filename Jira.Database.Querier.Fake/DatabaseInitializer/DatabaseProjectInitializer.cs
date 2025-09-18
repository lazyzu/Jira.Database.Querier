using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.Project.Fields;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Fake.DatabaseInitializer
{
    public class DatabaseProjectInitializer
    {
        public readonly FieldSelectionHandler FieldSelection = new FieldSelectionHandler();
        protected readonly FieldHandler Field = new FieldHandler();

        public async Task AddJiraProject(IJiraProject jiraProject, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
        {
            await jiraContext.project.AddAsync(new EntityFrameworkCore.Model.project
            {
                ID = jiraProject.Id,
                pname = jiraProject.Name,
                URL = jiraProject.Url,
                LEAD = jiraProject.Lead.Key,
                DESCRIPTION = jiraProject.Description,
                pkey = jiraProject.Key,
                pcounter = null,    // TODO: Not support yet
                ASSIGNEETYPE = null, // TODO: Not support yet
                AVATAR = jiraProject.Avatar.Id,
                ORIGINALKEY = jiraProject.Key,
                PROJECTTYPE = jiraProject.Type
            });

            await Field.RenderProjectKeyAssociationTable(jiraProject, jiraContext, saveChange: false);
            await Field.RenderProjectCategoryAssociationTable(jiraProject, jiraContext, saveChange: false);

            await Field.RenderProjectRoleActorMap(jiraProject, jiraProject.ProjectRoles, jiraContext, saveChange: false);
            await Field.RenderProjectIssueTypeAssociationTable(jiraProject, jiraProject.IssueTypeScheme, jiraContext, saveChange: false);
            await Field.RenderProjectComponentAssociationTable(jiraProject.Components, jiraContext, saveChange: false);
            await Field.RenderProjectVersionAssociationTable(jiraProject.Versions, jiraContext, saveChange: false);
            await Field.RenderProjectSecurityLevelSchemeLink(jiraProject, jiraContext, saveChange: false);

            if (saveChange) await jiraContext.SaveChangesAsync();
        }

        public class FieldSelectionHandler
        {
            public async Task AddIssueType(IEnumerable<IIssueType> issueTypes, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var issueType in issueTypes)
                {
                    await AddIssueType(issueType, jiraContext, saveChange: false);
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddIssueType(IIssueType issueType, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                jiraContext.issuetype.Add(new EntityFrameworkCore.Model.issuetype
                {
                    ID = issueType.Id,
                    SEQUENCE = null,  // TODO: Not support yet
                    pname = issueType.Name,
                    pstyle = issueType.IsSubTask ? "jira_subtask" : null,  // TODO: Not support yet
                    DESCRIPTION = issueType.Description,
                    ICONURL = null, // TODO: Not support yet
                    AVATAR = null,  // TODO: Not support yet
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddProjectRole(IEnumerable<IProjectRole> projectRoles, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var projectRole in projectRoles)
                {
                    await AddProjectRole(projectRole, jiraContext, saveChange: false);
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddProjectRole(IProjectRole projectRole, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.projectrole.AddAsync(new EntityFrameworkCore.Model.projectrole
                {
                    ID = projectRole.Id,
                    NAME = projectRole.Name,
                    DESCRIPTION = projectRole.Description
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddProjectCategory(IEnumerable<IProjectCategory> projectCategories, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var projectCategory in projectCategories) await AddProjectCategory(projectCategory, jiraContext, saveChange: false);

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddProjectCategory(IProjectCategory projectCategory, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.projectcategory.AddAsync(new EntityFrameworkCore.Model.projectcategory
                {
                    ID = projectCategory.Id,
                    cname = projectCategory.Name,
                    description = projectCategory.Description
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddSecurityLevelScheme(IEnumerable<JiraProjectFake.FieldSelectionGenerator.SecurityLevelScheme> schemes, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var scheme in schemes)
                {
                    await jiraContext.issuesecurityscheme.AddAsync(new EntityFrameworkCore.Model.issuesecurityscheme
                    {
                        ID = scheme.Scheme.Id,
                        NAME = scheme.Scheme.Name,
                        DESCRIPTION = scheme.Scheme.Description,
                        DEFAULTLEVEL = scheme.Scheme.DefaultValue.Id
                    });

                    foreach (var securityLevel in scheme.SecurityLevels)
                    {
                        await jiraContext.schemeissuesecuritylevels.AddAsync(new EntityFrameworkCore.Model.schemeissuesecuritylevels
                        {
                            ID = securityLevel.Id,
                            NAME = securityLevel.Name,
                            DESCRIPTION = securityLevel.Description,
                            SCHEME = securityLevel.Scheme.Id
                        });
                    }
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }
        }

        public class FieldHandler
        {
            protected long projectKeyIdIndex = 0;
            protected long issuetypescreenschemeentityIdIndex = 0;
            protected long projectroleactorIdIndex = 0;

            public async Task RenderProjectKeyAssociationTable(IJiraProject jiraProject, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.project_key.AddAsync(new EntityFrameworkCore.Model.project_key
                {
                    ID = projectKeyIdIndex++,
                    PROJECT_ID = jiraProject.Id,
                    PROJECT_KEY1 = jiraProject.Key
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task RenderProjectIssueTypeAssociationTable(IJiraProject jiraProject, IIssueTypeScheme issueTypeScheme, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.nodeassociation.AddAsync(new EntityFrameworkCore.Model.nodeassociation
                {
                    SOURCE_NODE_ID = jiraProject.Id,
                    SOURCE_NODE_ENTITY = "Project",
                    SINK_NODE_ID = issueTypeScheme.Id,
                    SINK_NODE_ENTITY = "IssueTypeScreenScheme",
                    ASSOCIATION_TYPE = "ProjectScheme"
                });

                foreach (var issueType in issueTypeScheme.IssueTypes)
                {
                    await jiraContext.issuetypescreenschemeentity.AddAsync(new EntityFrameworkCore.Model.issuetypescreenschemeentity
                    {
                        ID = issuetypescreenschemeentityIdIndex++,
                        ISSUETYPE = issueType.Id,
                        SCHEME = issueTypeScheme.Id,
                        FIELDSCREENSCHEME = null,   // TODO: Not support yet
                    });
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task RenderProjectRoleActorMap(IJiraProject jiraProject, IEnumerable<IProjectRoleActorMap> projectRoleActorMaps, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var projectRoleActorMap in jiraProject.ProjectRoles)
                    await AddProjectRoleActorMap(jiraProject, projectRoleActorMap, jiraContext, saveChange: false);

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddProjectRoleActorMap(IJiraProject jiraProject, IProjectRoleActorMap projectRoleActorMap, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var actor in projectRoleActorMap.Actors)
                {
                    await jiraContext.projectroleactor.AddAsync(new EntityFrameworkCore.Model.projectroleactor
                    {
                        ID = projectroleactorIdIndex++,
                        PID = jiraProject.Id,
                        PROJECTROLEID = projectRoleActorMap.Id,
                        ROLETYPE = actor.Type,
                        ROLETYPEPARAMETER = actor.Value
                    });
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task RenderProjectCategoryAssociationTable(IJiraProject jiraProject, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.nodeassociation.AddAsync(new EntityFrameworkCore.Model.nodeassociation
                {
                    SOURCE_NODE_ID = jiraProject.Id,
                    SOURCE_NODE_ENTITY = "Project",
                    SINK_NODE_ID = jiraProject.Category.Id,
                    SINK_NODE_ENTITY = "ProjectCategory",
                    ASSOCIATION_TYPE = "ProjectCategory",
                    SEQUENCE = null
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task RenderProjectComponentAssociationTable(IEnumerable<IFullProjectComponent> projectComponents, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var component in projectComponents) await AddProjectComponent(component, jiraContext, saveChange: false);

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddProjectComponent(IFullProjectComponent projectComponent, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.component.AddAsync(new EntityFrameworkCore.Model.component
                {
                    ID = projectComponent.Id,
                    PROJECT = projectComponent.Project.Id,
                    cname = projectComponent.Name,
                    description = projectComponent.Description,
                    URL = null,
                    LEAD = null, // TODO: Not support yet
                    ASSIGNEETYPE = null,    // TODO: Not support yet
                    ARCHIVED = projectComponent.Archived ? "Y" : null,
                    DELETED = projectComponent.Deleted ? "Y" : null
                });
            }

            public async Task RenderProjectVersionAssociationTable(IEnumerable<IFullProjectVersion> projectVersions, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var version in projectVersions) await AddProjectVersion(version, jiraContext, saveChange: false);

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task AddProjectVersion(IFullProjectVersion projectVersion, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                await jiraContext.projectversion.AddAsync(new EntityFrameworkCore.Model.projectversion
                {
                    ID = projectVersion.Id,
                    PROJECT = projectVersion.Project.Id,
                    vname = projectVersion.Name,
                    DESCRIPTION = projectVersion.Description,
                    SEQUENCE = null,
                    RELEASED = null,
                    ARCHIVED = projectVersion.Archived ? "true" : "",
                    URL = null,
                    STARTDATE = projectVersion.StartDate,
                    RELEASEDATE = projectVersion.ReleaseDate
                });

                if (saveChange) await jiraContext.SaveChangesAsync();
            }

            public async Task RenderProjectSecurityLevelSchemeLink(IJiraProject jiraProject, EntityFrameworkCore.JiraContext jiraContext, bool saveChange = true)
            {
                var securityLevelSchemeIds = jiraProject.SecurityLevels.Select(securityLevel => securityLevel.Scheme.Id).Distinct();

                foreach (var securityLevelSchemeId in securityLevelSchemeIds)
                {
                    await jiraContext.nodeassociation.AddAsync(new EntityFrameworkCore.Model.nodeassociation
                    {
                        SOURCE_NODE_ID = jiraProject.Id,
                        SOURCE_NODE_ENTITY = "Project",
                        SINK_NODE_ID = securityLevelSchemeId,
                        SINK_NODE_ENTITY = "IssueSecurityScheme",
                        ASSOCIATION_TYPE = "ProjectScheme",
                        SEQUENCE = null
                    });
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }
        }
    }
}
