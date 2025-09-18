using lazyzu.Jira.Database.EntityFrameworkCore;
using lazyzu.Jira.Database.Querier.User;
using lazyzu.Jira.Database.Querier.User.Fields;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Fake.DatabaseInitializer
{
    public class DatabaseUserInitializer
    {
        public readonly GroupSelectionHandler GroupSelection;

        protected readonly FieldHandler field;
        protected readonly MembershipIdCache membershipIdCache = new MembershipIdCache();

        protected readonly long directory_id = 0;

        public DatabaseUserInitializer()
        {
            this.GroupSelection = new GroupSelectionHandler(directory_id, membershipIdCache);
            this.field = new FieldHandler(membershipIdCache);
        }

        public virtual async Task AddUser(IJiraUser jiraUser, JiraContext jiraContext, bool saveChange = true)
        {
            await jiraContext.app_user.AddAsync(new EntityFrameworkCore.Model.app_user
            {
                ID = jiraUser.AppId,
                user_key = jiraUser.Key,
                lower_user_name = jiraUser.Username?.ToLower()
            });
            await jiraContext.cwd_user.AddAsync(new EntityFrameworkCore.Model.cwd_user
            {
                ID = jiraUser.CwdId,
                user_name = jiraUser.Username,
                lower_user_name = jiraUser.Username?.ToLower(),
                display_name = jiraUser.DisplayName,
                lower_display_name = jiraUser.DisplayName?.ToLower(),
                email_address = jiraUser.Email,
                lower_email_address = jiraUser.Email?.ToLower(),
                directory_id = directory_id,
                active = jiraUser.IsActive ?? false ? 1 : 0,
            });
            await field.RenderAvatarAssociationTable(jiraUser, jiraContext);
            await field.RenderGroupAssociationTable(jiraUser, jiraContext, directory_id);

            if (saveChange) await jiraContext.SaveChangesAsync();
        }

        public class GroupSelectionHandler
        {
            protected readonly long directory_id;
            protected readonly MembershipIdCache membershipIdCache;

            public GroupSelectionHandler(long directory_id, MembershipIdCache membershipIdCache)
            {
                this.directory_id = directory_id;
                this.membershipIdCache = membershipIdCache;
            }

            public virtual async Task InitGroups(IEnumerable<IUserGroup> parentGroups, IEnumerable<IUserGroup> cascadingChildGroups, JiraContext jiraContext, bool saveChange = true)
            {
                foreach (var cascadingChildGroup in cascadingChildGroups)
                {
                    var parentGroup = parentGroups.First(_parentGroup => cascadingChildGroup.Name!.StartsWith(_parentGroup.Name));
                    await jiraContext.cwd_membership.AddAsync(new EntityFrameworkCore.Model.cwd_membership
                    {
                        ID = membershipIdCache.cwd_membership_id++,
                        parent_id = parentGroup!.Id,
                        child_id = cascadingChildGroup.Id,
                        child_name = cascadingChildGroup.Name,
                        lower_child_name = cascadingChildGroup.Name?.ToLower(),
                        parent_name = parentGroup.Name,
                        lower_parent_name = parentGroup.Name?.ToLower(),
                        directory_id = directory_id,
                        membership_type = "GROUP_GROUP"
                    });
                }

                if (saveChange) await jiraContext.SaveChangesAsync();
            }
        }

        public class FieldHandler
        {
            protected long avatarPropId = 0;
            protected readonly MembershipIdCache membershipIdCache;

            public FieldHandler(MembershipIdCache membershipIdCache)
            {
                this.membershipIdCache = membershipIdCache;
            }

            public virtual async Task RenderAvatarAssociationTable(IJiraUser jiraUser, JiraContext jiraContext)
            {
                var currentAvatarPropId = avatarPropId++;
                await jiraContext.propertyentry.AddAsync(new EntityFrameworkCore.Model.propertyentry
                {
                    ID = currentAvatarPropId,
                    ENTITY_ID = jiraUser.AppId,
                    ENTITY_NAME = "ApplicationUser",
                    PROPERTY_KEY = "user.avatar.id",
                });

                await jiraContext.propertynumber.AddAsync(new EntityFrameworkCore.Model.propertynumber
                {
                    ID = currentAvatarPropId,
                    propertyvalue = jiraUser.Avatar.Id
                });
            }

            public virtual async Task RenderGroupAssociationTable(IJiraUser jiraUser, JiraContext jiraContext, decimal directory_id)
            {
                if (jiraUser.Groups != null)
                {
                    foreach (var parentGroup in jiraUser.Groups)
                    {
                        var isParentGroupOfUser = jiraUser.Groups.Any(x => x.Id != parentGroup.Id && x.Name.StartsWith(parentGroup.Name!)) == false;
                        if (isParentGroupOfUser) await jiraContext.cwd_membership.AddAsync(new EntityFrameworkCore.Model.cwd_membership
                        {
                            ID = membershipIdCache.cwd_membership_id++,
                            parent_id = parentGroup.Id,
                            child_id = jiraUser.CwdId,
                            child_name = jiraUser.Username,
                            lower_child_name = jiraUser.Username?.ToLower(),
                            parent_name = parentGroup.Name,
                            lower_parent_name = parentGroup.Name?.ToLower(),
                            directory_id = directory_id,
                            membership_type = "GROUP_USER"
                        });

                        var parentOf_ParentGroup = jiraUser.Groups
                            .Where(parentCandidate => parentCandidate.Id != parentGroup.Id
                                                   && parentGroup.Name!.StartsWith(parentCandidate.Name))
                            .OrderByDescending(parentCandidate => parentCandidate.Name.Length)
                            .FirstOrDefault();

                        var hasParentOfThisGroup = parentOf_ParentGroup != null;

                        if (hasParentOfThisGroup) await jiraContext.cwd_membership.AddAsync(new EntityFrameworkCore.Model.cwd_membership
                        {
                            ID = membershipIdCache.cwd_membership_id++,
                            parent_id = parentOf_ParentGroup!.Id,
                            child_id = parentGroup.Id,
                            child_name = parentGroup.Name,
                            lower_child_name = parentGroup.Name?.ToLower(),
                            parent_name = parentOf_ParentGroup.Name,
                            lower_parent_name = parentOf_ParentGroup.Name?.ToLower(),
                            directory_id = directory_id,
                            membership_type = "GROUP_GROUP"
                        });
                    }
                }
            }
        }

        public class MembershipIdCache
        {
            public long cwd_membership_id = 0;
        }
    }
}
