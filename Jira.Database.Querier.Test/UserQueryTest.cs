using lazyzu.Jira.Database.Querier.Test.TestContext;
using lazyzu.Jira.Database.Querier.User;
using lazyzu.Jira.Database.Querier.User.Fields;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Test
{
    internal class UserQueryTest : IDisposable
    {
        private readonly InMemoryTestContext testContext;

        public UserQueryTest()
        {
            this.testContext = new InMemoryTestContext();
            this.testContext.InitUserContext().Wait();
        }

        [Test]
        public async Task QueryUserByKey_DefaultField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                foreach (var goldenUser in await testContext.GenerateUsers(1))
                {
                    var userInfo = await jiraDatabaseQuerier.User.GetUserByKeyAsync(goldenUser.Key);
                    CheckProps(userInfo, goldenUser, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        [Test]
        public async Task QueryUserByName_DefaultField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                foreach (var goldenUser in await testContext.GenerateUsers(1))
                {
                    var userInfo = await jiraDatabaseQuerier.User.GetUserByNameAsync(goldenUser.Username);
                    CheckProps(userInfo, goldenUser, jiraDatabaseQuerier.User.DefaultQueryFields);
                }
            });
        }

        [Test]
        public async Task QueryUserByName_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                foreach (var goldenUser in await testContext.GenerateUsers(1))
                {
                    var userInfo = await jiraDatabaseQuerier.User.GetUserByNameAsync(goldenUser.Username, fields: UserFieldSelection.All.ToArray());
                    CheckProps(userInfo, goldenUser, UserFieldSelection.All);
                }
            });
        }

        [Test]
        public async Task QueryUsersByKey_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var goldenUsers = await testContext.GenerateUsers(1);

                var userKeys = goldenUsers.Select(user => user.Key).ToArray();
                var userInfos = await jiraDatabaseQuerier.User.GetUsersByKeyAsync(userKeys, fields: UserFieldSelection.All.ToArray());

                foreach (var goldenUser in goldenUsers)
                {
                    var matchedUserInfo = userInfos.FirstOrDefault(user => user.Username == goldenUser.Username);

                    if (matchedUserInfo == null) Assert.Fail("user is missing");
                    else CheckProps(matchedUserInfo, goldenUser, UserFieldSelection.All);
                }
            });
        }

        [Test]
        public async Task QueryUsersByName_AllField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var goldenUsers = await testContext.GenerateUsers(1);

                var userNames = goldenUsers.Select(user => user.Username).ToArray();
                var userInfos = await jiraDatabaseQuerier.User.GetUsersByNameAsync(userNames, fields: UserFieldSelection.All.ToArray());

                foreach (var goldenUser in goldenUsers)
                {
                    var matchedUserInfo = userInfos.FirstOrDefault(user => user.Username == goldenUser.Username);

                    if (matchedUserInfo == null) Assert.Fail("user is missing");
                    else CheckProps(matchedUserInfo, goldenUser, UserFieldSelection.All);
                }
            });
        }

        public static void CheckProps(IJiraUser actual, IJiraUser expected, IEnumerable<User.Contract.FieldKey> checkFields)
        {
            if (checkFields == null) Assert.Fail("No any check fields");
            else
            {
                var _checkFields = checkFields.ToArray();

                if (_checkFields.Contains(UserFieldSelection.UserAppId)) Assert.That(actual.AppId, Is.EqualTo(expected.AppId));
                if (_checkFields.Contains(UserFieldSelection.UserCwdId)) Assert.That(actual.CwdId, Is.EqualTo(expected.CwdId));
                if (_checkFields.Contains(UserFieldSelection.UserKey)) Assert.That(actual.Key, Is.EqualTo(expected.Key));
                if (_checkFields.Contains(UserFieldSelection.UserName)) Assert.That(actual.Username, Is.EqualTo(expected.Username));
                if (_checkFields.Contains(UserFieldSelection.UserDisplayName)) Assert.That(actual.DisplayName, Is.EqualTo(expected.DisplayName));
                if (_checkFields.Contains(UserFieldSelection.UserEmail)) Assert.That(actual.Email, Is.EqualTo(expected.Email));
                if (_checkFields.Contains(UserFieldSelection.UserActive)) Assert.That(actual.IsActive, Is.EqualTo(expected.IsActive));
                if (_checkFields.Contains(UserFieldSelection.UserAvatar)) AssertUtil.MemberwisePropertiesEqual<UserAvatar>(actual.Avatar, expected.Avatar);
                if (_checkFields.Contains(UserFieldSelection.UserGroup))AssertUtil.EquivalentToAndMemberwisePropertiesEqual<UserGroup>(actual.Groups, expected.Groups, group => group.Id);
            }
        }

        public void Dispose()
        {
            testContext.Dispose();
        }
    }
}