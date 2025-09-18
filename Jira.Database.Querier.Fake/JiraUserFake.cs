using Bogus;
using lazyzu.Jira.Database.Querier.Avatar;
using lazyzu.Jira.Database.Querier.User;
using lazyzu.Jira.Database.Querier.User.Fields;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.Fake
{
    public class JiraUserFake
    {
        public ImmutableArray<IUserGroup> ParentGroups { get; protected set; }
        public ImmutableArray<IUserGroup> CascadingChildGroups { get; protected set; }
        public ImmutableArray<IUserGroup> Groups { get; protected set; }

        public readonly GroupSelectionHandler GroupSelection = new GroupSelectionHandler();

        protected readonly string jiraServerUrl;

        protected int appIdIndex = 0;
        protected int cwdIdIndex = 0;

        public JiraUserFake(ConstructArgument args = null)
        {
            var _args = args ?? ConstructArgument.Default;

            jiraServerUrl = _args.JiraServerUrl;

            ParentGroups = _args.ParentGroups.IsEmpty ? GroupSelection.GenerateParentGroup(_args.ParentGroupCount).ToImmutableArray() : _args.ParentGroups;
            CascadingChildGroups = _args.CascadingChildGroups.IsEmpty ? GroupSelection.GenerateCascadingChildGroup(_args.CascadingChildGroupCount, ParentGroups).ToImmutableArray() : _args.CascadingChildGroups;
            Groups = ParentGroups.Concat(CascadingChildGroups).ToImmutableArray();
        }

        public IJiraUser[] Generate(int count)
        {
            var fakedGroups = ParentGroups.Concat(CascadingChildGroups).ToArray();

            var userFake = new Faker<JiraUser>()
                .StrictMode(true)
                .RuleFor(user => user.AppId, faker => appIdIndex++)
                .RuleFor(user => user.CwdId, faker => cwdIdIndex++)
                .RuleFor(user => user.Key, (faker, user) => $"JIRAUSER{user.AppId}")
                .RuleFor(user => user.Username, faker => $"{faker.Name.FirstName()}_{faker.Name.LastName()}")
                .RuleFor(user => user.DisplayName, (faker, user) => user.Username)
                .RuleFor(user => user.Email, (faker, user) => $"{user.Username}@testlazyzu.Jira.com")
                .RuleFor(user => user.IsActive, true)
                .RuleFor(user => user.Avatar, (faker, user) => new UserAvatar
                {
                    Id = user.AppId,
                    Urls = new AvatarUrl
                    {
                        Large = $"{jiraServerUrl}/secure/useravatar?avatarId={user.AppId}",
                        Medium = $"{jiraServerUrl}/secure/useravatar?size=medium&avatarId={user.AppId}",
                        Small = $"{jiraServerUrl}/secure/useravatar?size=small&avatarId={user.AppId}",
                        XSmall = $"{jiraServerUrl}/secure/useravatar?size=xsmall&avatarId={user.AppId}"
                    }
                })
                .RuleFor(user => user.Groups, (faker, user) =>
                {
                    var pickeds = faker.PickRandom(fakedGroups, 10).ToArray();
                    var cascadingBaseGroup = fakedGroups.Where(group => pickeds.Any(picked => picked.Name.StartsWith(group.Name))).ToArray();
                    return pickeds.Concat(cascadingBaseGroup).ToHashSet<IUserGroup>();
                });

            return userFake.Generate(count).ToArray();
        }

        public class GroupSelectionHandler
        {
            protected Faker<UserGroup> parentGroupFake;

            protected int groupIdIndex = 0;

            public GroupSelectionHandler()
            {
                this.parentGroupFake = BuildParentGroupFake();
            }

            public virtual IEnumerable<IUserGroup> GenerateParentGroup(int count)
                => parentGroupFake.Generate(count);

            protected virtual Faker<UserGroup> BuildParentGroupFake()
            {
                return new Faker<UserGroup>()
                .StrictMode(true)
                .RuleFor(group => group.Id, faker => groupIdIndex++)
                .RuleFor(group => group.Name, faker => faker.Random.Word());
            }

            public virtual IEnumerable<IUserGroup> GenerateCascadingChildGroup(int count, IEnumerable<IUserGroup> parentGroups)
            {
                var faker = BuildCascadingChildGroupFake(parentGroups);
                return faker.Generate(count);
            }

            protected virtual Faker<UserGroup> BuildCascadingChildGroupFake(IEnumerable<IUserGroup> parentGroups)
            {
                return new Faker<UserGroup>()
                .StrictMode(true)
                .RuleFor(group => group.Id, faker => groupIdIndex++)
                .RuleFor(group => group.Name, faker => $"{faker.PickRandom(parentGroups.AsEnumerable()).Name}-{faker.Random.Word()}");
            }
        }

        public class ConstructArgument
        {
            public string JiraServerUrl { get; set; } = Constants.JiraServerUrl;

            public ImmutableArray<IUserGroup> ParentGroups { get; set; } = ImmutableArray<IUserGroup>.Empty;
            public ImmutableArray<IUserGroup> CascadingChildGroups { get; set; } = ImmutableArray<IUserGroup>.Empty;

            public int ParentGroupCount { get; set; } = 10;
            public int CascadingChildGroupCount { get; set; } = 10;


            public static readonly ConstructArgument Default = new ConstructArgument();
        }
    }
}
