using Bogus;
using lazyzu.Jira.Database.Querier.Avatar;
using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Project;
using lazyzu.Jira.Database.Querier.Project.Fields;
using lazyzu.Jira.Database.Querier.User;
using lazyzu.Jira.Database.Querier.User.Fields;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.Fake
{
    public class JiraProjectFake
    {
        public ImmutableArray<IIssueType> IssueTypeSelections { get; protected set; }

        public ImmutableArray<IProjectCategory> ProjectCategorySelections { get; protected set; }
        public ImmutableArray<IProjectRole> ProjectRoleSelections { get; protected set; }
        public ImmutableArray<FieldSelectionGenerator.SecurityLevelScheme> SecurityLevelSchemeSelections { get; protected set; }

        public readonly FieldSelectionGenerator FieldSelection;
        protected readonly FieldFake Filed;

        protected readonly string jiraServerUrl;
        protected long projectIdIndex = 0;
        protected FieldSelectionGenerator.SecurityLevelScheme.IndexCache securityLevelSchemeIndexCache = new FieldSelectionGenerator.SecurityLevelScheme.IndexCache();

        public JiraProjectFake(ConstructArgument args = null)
        {
            var _args = args ?? ConstructArgument.Default;

            jiraServerUrl = _args.JiraServerUrl;

            FieldSelection = new FieldSelectionGenerator();

            IssueTypeSelections = _args.IssueTypeSelections.IsEmpty ? FieldSelection.GenerateIssueTypes().ToImmutableArray() : _args.IssueTypeSelections;

            ProjectCategorySelections = _args.ProjectCategorySelections.IsEmpty ? FieldSelection.GenerateProjectCategory(_args.ProjectCategorySelectionCount).Cast<IProjectCategory>().ToImmutableArray() : _args.ProjectCategorySelections;
            ProjectRoleSelections = _args.ProjectRoleSelections.IsEmpty ? FieldSelection.GenerateProjectRoles().ToImmutableArray() : _args.ProjectRoleSelections;
            SecurityLevelSchemeSelections = _args.SecurityLevelSchemeSelections.IsEmpty ? FieldSelectionGenerator.SecurityLevelScheme.GenerateSecurityLevelScheme(_args.SecurityLevelSchemeSelectionCount, securityLevelSchemeIndexCache).ToImmutableArray() : _args.SecurityLevelSchemeSelections;
            
            Filed = new FieldFake(IssueTypeSelections);
        }

        public IJiraProject[] Generate(int count, GenerateArgument args)
        {
            var _users = args.Users.ToArray();
            var _groups = args.Groups.ToArray();

            var projectFake = new Faker<JiraProject>()
                .StrictMode(true)
                .RuleFor(project => project.Id, faker => projectIdIndex++)
                .RuleFor(project => project.Name, faker => faker.Name.JobTitle())
                .RuleFor(project => project.Url, (faker, project) => string.Empty)
                .RuleFor(project => project.Lead, faker => faker.PickRandom(_users))
                .RuleFor(project => project.Description, (faker, project) => $"{project.Name} Project Description")
                .RuleFor(project => project.Key, (faker, project) => project.Id.ToString())
                .RuleFor(project => project.Avatar, faker =>
                {
                    var avatarId = faker.Random.Int();
                    return new ProjectAvatar
                    {
                        Id = avatarId,
                        Urls = new AvatarUrl
                        {
                            Large = $"{jiraServerUrl}/secure/projectavatar?avatarId={avatarId}",
                            Medium = $"{jiraServerUrl}/secure/projectavatar?size=medium&avatarId={avatarId}",
                            Small = $"{jiraServerUrl}/secure/projectavatar?size=small&avatarId={avatarId}",
                            XSmall = $"{jiraServerUrl}/secure/projectavatar?size=xsmall&avatarId={avatarId}"
                        }
                    };
                })
                .RuleFor(project => project.Type, (faker, project) => $"Type of {project.Name}")
                .RuleFor(project => project.Category, faker => faker.PickRandom(ProjectCategorySelections.AsEnumerable()))
                .RuleFor(project => project.ProjectRoles, faker => Filed.GenerateProjectRoleActorMap(_users, _groups, ProjectRoleSelections).ToArray())
                .RuleFor(project => project.IssueTypeScheme, faker => Filed.GenerateProjectIssueTypeScheme())
                .RuleFor(project => project.Components, (faker, project) => Filed.GenerateProjectComponent(faker.Random.Int(args.ProjectComponentCount.Min, args.ProjectComponentCount.Max))
                    .Select(projectComponent => new FullProjectComponent
                    {
                        Id = projectComponent.Id,
                        Name = projectComponent.Name,
                        Description = projectComponent.Description,
                        Archived = projectComponent.Archived,
                        Deleted = projectComponent.Deleted,
                        Project = project
                    }).ToArray())
                .RuleFor(project => project.Versions, (faker, project) => Filed.GenerateProjectVersion(faker.Random.Int(args.ProjectVersionCount.Min, args.ProjectVersionCount.Max))
                    .Select(projectVersion => new FullProjectVersion(projectVersion, project)).ToArray())
                .RuleFor(project => project.SecurityLevels, faker => faker.PickRandom(SecurityLevelSchemeSelections.AsEnumerable()).SecurityLevels);

            return projectFake.Generate(count).ToArray();
        }

        public class FieldSelectionGenerator
        {
            protected readonly Faker<ProjectCategory> projectCategoryFake;

            public FieldSelectionGenerator()
            {
                this.projectCategoryFake = BuildProjectCategoryFake();
            }

            public virtual IEnumerable<IIssueType> GenerateIssueTypes()
            {
                var id = 0;

                const string story = "Story";
                yield return new IssueType
                {
                    Id = (id++).ToString(),
                    Name = story,
                    Description = $"{story} Description",
                    IsSubTask = false,
                };

                const string task = "Task";
                yield return new IssueType
                {
                    Id = (id++).ToString(),
                    Name = task,
                    Description = $"{task} Description",
                    IsSubTask = false,
                };

                const string bug = "Bug";
                yield return new IssueType
                {
                    Id = (id++).ToString(),
                    Name = bug,
                    Description = $"{bug} Description",
                    IsSubTask = false,
                };

                const string epic = "Epic";
                yield return new IssueType
                {
                    Id = (id++).ToString(),
                    Name = epic,
                    Description = $"{epic} Description",
                    IsSubTask = false,
                };

                const string subTask = "sub-Task";
                yield return new IssueType
                {
                    Id = (id++).ToString(),
                    Name = subTask,
                    Description = $"{subTask} Description",
                    IsSubTask = false,
                };
            }

            public virtual IEnumerable<IProjectCategory> GenerateProjectCategory(int count)
                => projectCategoryFake.Generate(count);

            protected virtual Faker<ProjectCategory> BuildProjectCategoryFake()
            {
                return new Faker<ProjectCategory>()
                    .StrictMode(true)
                    .RuleFor(category => category.Id, faker => faker.IndexFaker)
                    .RuleFor(category => category.Name, faker => faker.Name.JobType())
                    .RuleFor(category => category.Description, (faker, category) => $"{category.Name} Description");
            }

            public virtual IEnumerable<IProjectRole> GenerateProjectRoles()
            {
                var id = 0;

                const string maintainer = "Maintainer";
                yield return new ProjectRole
                {
                    Id = id++,
                    Name = maintainer,
                    Description = $"{maintainer} Description"
                };

                const string developer = "Developer";
                yield return new ProjectRole
                {
                    Id = id++,
                    Name = developer,
                    Description = $"{developer} Description"
                };

                const string reporter = "Reporter";
                yield return new ProjectRole
                {
                    Id = id++,
                    Name = reporter,
                    Description = $"{reporter} Description"
                };

                const string guest = "Guest";
                yield return new ProjectRole
                {
                    Id = id++,
                    Name = guest,
                    Description = $"{guest} Description"
                };
            }

            public class SecurityLevelScheme
            {
                public IssueSecurityLevelScheme Scheme { get; init; }
                public IssueSecurityLevel[] SecurityLevels { get; init; }

                private static readonly Bogus.Faker faker = new Bogus.Faker();

                public static IEnumerable<SecurityLevelScheme> GenerateSecurityLevelScheme(int count, IndexCache indexCache)
                {
                    for (int i = 0; i < count; i++)
                    {
                        yield return GenerateSecurityLevelScheme(indexCache);
                    }
                }

                public static SecurityLevelScheme GenerateSecurityLevelScheme(IndexCache indexCache)
                {
                    var securityLevelSelections = new string[] { "private", "protected", "internal", "public" };

                    var pickedSecurityLevelNames = faker.PickRandom(securityLevelSelections, faker.Random.Int(1, securityLevelSelections.Length));

                    var pickedSecurityLevelsWithoutScheme = pickedSecurityLevelNames.Select(name => new
                    {
                        Id = indexCache.SecurityLevelIdIndex++,
                        Name = name,
                        Description = $"{name} Description",
                    }).ToArray();

                    var schemeDefaultSecurityLevel = faker.PickRandom(pickedSecurityLevelsWithoutScheme);
                    var schemeName = faker.Lorem.Word();
                    var scheme = new IssueSecurityLevelScheme(id: indexCache.SecurityLevelSchemeIndex++
                        , name: schemeName
                        , description: $"{schemeName} Description"
                        , defaultValue: (id: schemeDefaultSecurityLevel.Id
                                       , name: schemeDefaultSecurityLevel.Name
                                       , description: schemeDefaultSecurityLevel.Description));

                    var securityLevels = pickedSecurityLevelsWithoutScheme.Select(securityLevel => new IssueSecurityLevel
                    {
                        Id = securityLevel.Id,
                        Name = securityLevel.Name,
                        Description = securityLevel.Description,
                        Scheme = scheme
                    }).ToArray();

                    return new SecurityLevelScheme
                    {
                        Scheme = scheme,
                        SecurityLevels = securityLevels
                    };
                }

                public class IndexCache
                {
                    public int SecurityLevelSchemeIndex;
                    public int SecurityLevelIdIndex;
                }
            }
        }

        public class FieldFake
        {
            protected readonly Faker<IssueTypeScheme> issueTypeSchemeFake;
            protected readonly Faker<ProjectComponent> projectComponentFake;
            protected readonly Faker<ProjectVersion> projectVersionFake;

            public FieldFake(ImmutableArray<IIssueType> issueTypeSelections)
            {
                this.issueTypeSchemeFake = BuildProjectIssueTypeSchemeFake(issueTypeSelections);
                this.projectComponentFake = BuildProjectComponentFake();
                this.projectVersionFake = BuildProjectVersionFake();
            }

            public virtual IEnumerable<IProjectRoleActorMap> GenerateProjectRoleActorMap(IJiraUser[] users
            , IUserGroup[] groups
            , ImmutableArray<IProjectRole> ProjectRoleSelections)
            {
                foreach (var roleSelection in ProjectRoleSelections)
                {
                    yield return new ProjectRoleActorMap
                    {
                        Id = roleSelection.Id,
                        Name = roleSelection.Name,
                        Description = $"{roleSelection.Name} Description",
                        Actors = GenerateProjectRoleActor(users, groups)
                    };
                }
            }

            protected virtual HashSet<IProjectRoleActor> GenerateProjectRoleActor(IJiraUser[] users, IUserGroup[] groups)
            {
                var faker = new Bogus.Faker();

                var result = new HashSet<IProjectRoleActor>();
                var pickedUsers = faker.PickRandom(users, faker.Random.Int(1, users.Length));
                foreach (var user in pickedUsers)
                {
                    result.Add(new ProjectRoleActor
                    {
                        Type = IProjectRoleActor.TypeSelection.AtlassianUserRoleActor,
                        Value = user.Key
                    });
                }

                var pickedGroups = faker.PickRandom(groups, faker.Random.Int(1, groups.Length));
                foreach (var group in pickedGroups)
                {
                    result.Add(new ProjectRoleActor
                    {
                        Type = IProjectRoleActor.TypeSelection.AtlassianGroupRoleActor,
                        Value = group.Name
                    });
                }

                return result;
            }

            public virtual IEnumerable<IssueTypeScheme> GenerateProjectIssueTypeScheme(int count)
                => issueTypeSchemeFake.Generate(count);

            public virtual IssueTypeScheme GenerateProjectIssueTypeScheme()
                => issueTypeSchemeFake.Generate();

            protected virtual Faker<IssueTypeScheme> BuildProjectIssueTypeSchemeFake(ImmutableArray<IIssueType> issueTypeSelections)
            {
                return new Faker<IssueTypeScheme>()
                    .StrictMode(true)
                    .RuleFor(issueTypeScheme => issueTypeScheme.Id, faker => faker.IndexFaker)
                    .RuleFor(issueTypeScheme => issueTypeScheme.IssueTypes, faker =>
                    {
                        return faker.PickRandom(issueTypeSelections.AsEnumerable()
                            , amountToPick: faker.Random.Int(1, issueTypeSelections.Length))
                        .ToArray();
                    });
            }

            public virtual IEnumerable<IProjectComponent> GenerateProjectComponent(int count)
                => projectComponentFake.Generate(count);

            protected virtual Faker<ProjectComponent> BuildProjectComponentFake()
            {
                return new Faker<ProjectComponent>()
                    .StrictMode(true)
                    .RuleFor(projectComponent => projectComponent.Id, faker => faker.IndexFaker)
                    .RuleFor(projectComponent => projectComponent.Name, faker => faker.Lorem.Word())
                    .RuleFor(projectComponent => projectComponent.Description, (faker, projectComponent) => $"{projectComponent.Name} Description")
                    .RuleFor(projectComponent => projectComponent.Archived, faker => faker.PickRandom(new bool[] { true, false }))
                    .RuleFor(projectComponent => projectComponent.Deleted, faker => false);
            }

            public virtual IEnumerable<IProjectVersion> GenerateProjectVersion(int count)
                => projectVersionFake.Generate(count);

            protected virtual Faker<ProjectVersion> BuildProjectVersionFake()
            {
                var booleanSelection = new bool[] { true, false };

                return new Faker<ProjectVersion>()
                    .StrictMode(true)
                    .RuleFor(projectVersion => projectVersion.Id, faker => faker.IndexFaker)
                    .RuleFor(projectVersion => projectVersion.Name, faker => faker.Lorem.Word())
                    .RuleFor(projectVersion => projectVersion.Description, (faker, projectVersion) => $"{projectVersion.Name} Description")
                    .RuleFor(projectVersion => projectVersion.Archived, faker => faker.PickRandom(booleanSelection))
                    .RuleFor(projectVersion => projectVersion.StartDate, faker => faker.Date.Past())
                    .RuleFor(projectVersion => projectVersion.ReleaseDate, (faker, projectVersion) =>
                    {
                        var isReleased = faker.PickRandom(booleanSelection);
                        if (isReleased) return faker.Date.Future(refDate: projectVersion.StartDate);
                        else return null;
                    });
            }
        }

        public class ConstructArgument
        {
            public string JiraServerUrl { get; set; } = Constants.JiraServerUrl;

            public ImmutableArray<IIssueType> IssueTypeSelections { get; set; } = ImmutableArray<IIssueType>.Empty;

            public ImmutableArray<IProjectCategory> ProjectCategorySelections { get; set; } = ImmutableArray<IProjectCategory>.Empty;
            public ImmutableArray<IProjectRole> ProjectRoleSelections { get; set; } = ImmutableArray<IProjectRole>.Empty;
            public ImmutableArray<FieldSelectionGenerator.SecurityLevelScheme> SecurityLevelSchemeSelections { get; set; } = ImmutableArray<FieldSelectionGenerator.SecurityLevelScheme>.Empty;

            public int ProjectCategorySelectionCount { get; set; } = 3;
            public int SecurityLevelSchemeSelectionCount { get; set; } = 3;

            public static readonly ConstructArgument Default = new ConstructArgument();
        }

        public class GenerateArgument
        {
            public IEnumerable<IJiraUser> Users { get; set; }
            public IEnumerable<IUserGroup> Groups { get; set; }

            public RandomRange ProjectComponentCount { get; set; } = new RandomRange(0, 10);
            public RandomRange ProjectVersionCount { get; set; } = new RandomRange(0, 10);

            public GenerateArgument(IEnumerable<IJiraUser> users
                , IEnumerable<IUserGroup> groups)
            {
                this.Users = users;
                this.Groups = groups;
            }
        }
    }
}
