using lazyzu.Jira.Database.Querier.Fake;
using lazyzu.Jira.Database.Querier.Issue.Contract;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;
using lazyzu.Jira.Database.Querier.Test.TestContext;
using lazyzu.Jira.Database.Querier.User;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lazyzu.Jira.Database.Querier.Test
{
    internal class IssueCustomFieldTest : IDisposable
    {
        private readonly InMemoryTestContext testContext;

        public IssueCustomFieldTest()
        {
            this.testContext = new InMemoryTestContext();
            this.testContext.InitIssueContext().Wait();
        }

        [Test]
        public async Task CascadingSelectCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var cascadingSelectFieldKey = BuildCustomFieldKey<CascadingSelectCustomFieldSchema>("Cascading Select");
                var expectedValue = new CascadingSelectCustomFieldSchema
                {
                    Value = new CascadingSelection
                    {
                        CascadingSelections = new ISelectOption[]
                        {
                            BuildOption("cascading-select:1"),
                            BuildOption("cascading-select:2"),
                        }
                    }
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(cascadingSelectFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    cascadingSelectFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(cascadingSelectFieldKey, out var actualValue))
                {
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task DateTimeCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var dateTimeFieldKey = BuildCustomFieldKey<DateTimeCustomFieldSchema>("DateTime");
                var expectedValue = new DateTimeCustomFieldSchema
                {
                    Value = DateTime.Now,
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(dateTimeFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    dateTimeFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(dateTimeFieldKey, out var actualValue))
                {
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task LabelCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var labelFieldKey = BuildCustomFieldKey<LabelCustomFieldSchema>("Label");
                var expectedValue = new LabelCustomFieldSchema
                {
                    Value = Enumerable.Range(1, 5).Select(num => num.ToString()).ToHashSet()
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(labelFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    labelFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(labelFieldKey, out var actualValue))
                {
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task MultiSelectCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var multiSelectFieldKey = BuildCustomFieldKey<MultiSelectCustomFieldSchema>("Multi Select");
                var expectedValue = new MultiSelectCustomFieldSchema
                {
                    Value = new SelectCollection
                    {
                        Selections = new HashSet<ISelectOption>(new ISelectOption[]
                        {
                            BuildOption("multi-select:1"),
                            BuildOption("multi-select:2"),
                        })
                    }
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(multiSelectFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    multiSelectFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(multiSelectFieldKey, out var actualValue))
                {
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task MultiUserCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var multiUserFieldKey = BuildUserCustomFieldKey<MultiUserCustomFieldSchema>("Multi User", UserFieldSelection.All.ToArray());
                var expectedValue = new MultiUserCustomFieldSchema
                {
                    Value = new JiraUserCollection
                    {
                        Users = new HashSet<IJiraUser>(new IJiraUser[]
                        {
                             referenceUsers.First(),
                             referenceUsers.Last()
                        })
                    }
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(multiUserFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    multiUserFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(multiUserFieldKey, out var actualValue))
                {
                    AssertUtil.EquivalentToAndMemberwisePropertiesEqual<User.JiraUser>(actualValue.Value.Users
                        , expectedValue.Value.Users
                        , user => user.Username
                        , (actual, expected) =>
                    {
                        UserQueryTest.CheckProps(actual, expected, UserFieldSelection.All);
                    });
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task NumberCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var numberFieldKey = BuildCustomFieldKey<NumberCustomFieldSchema>("Number");
                var expectedValue = new NumberCustomFieldSchema
                {
                    Value = 1
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(numberFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    numberFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(numberFieldKey, out var actualValue))
                {
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task SelectCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var selectFieldKey = BuildCustomFieldKey<SelectCustomFieldSchema>("Select");
                var expectedValue = new SelectCustomFieldSchema
                {
                    Value = BuildOption("select")
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(selectFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    selectFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(selectFieldKey, out var actualValue))
                {
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task StringCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var stringFieldKey = BuildCustomFieldKey<StringCustomFieldSchema>("String");
                var expectedValue = new StringCustomFieldSchema
                {
                    Value = "Hello String"
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(stringFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    stringFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(stringFieldKey, out var actualValue))
                {
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task TextCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var textFieldKey = BuildCustomFieldKey<TextCustomFieldSchema>("Text");
                var expectedValue = new TextCustomFieldSchema
                {
                    Value = "Hello Text"
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(textFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    textFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(textFieldKey, out var actualValue))
                {
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
                else Assert.Fail("Field is not readount");
            });
        }

        [Test]
        public async Task UserCustomField()
        {
            await testContext.TestWithDatabase(async jiraDatabaseQuerier =>
            {
                var referenceUsers = await testContext.GenerateUsers(5);
                var referenceProjects = await testContext.GenerateProjects(5, new InMemoryTestContext.ProjectGenerateArgument(referenceUsers));

                var userFieldKey = BuildUserCustomFieldKey<UserCustomFieldSchema>("User", UserFieldSelection.All.ToArray());
                var expectedValue = new UserCustomFieldSchema
                {
                    Value = referenceUsers.First()
                };

                var generatedIssue = await testContext.GenerateIssue(new JiraIssueFake.GenerateArgument(referenceProjects, referenceUsers)
                {
                    RenderCustomField = (customFields, issue, faker) =>
                    {
                        customFields.Add(userFieldKey, expectedValue);
                    }
                });

                var issueInfo = await jiraDatabaseQuerier.Issue.GetIssueAsync(generatedIssue.Id, new FieldKey[]
                {
                    userFieldKey
                });

                if (issueInfo.CustomFields.TryGetValue(userFieldKey, out var actualValue))
                {
                    UserQueryTest.CheckProps(actualValue.Value, expectedValue.Value, UserFieldSelection.All);
                }
                else Assert.Fail("Field is not readount");
            });
        }


        private int fieldIdIndex = 0;
        private CustomFieldKey<TFieldScheme> BuildCustomFieldKey<TFieldScheme>(string fieldName)
        {
            return new CustomFieldKey<TFieldScheme>(fieldName, fieldIdIndex++);
        }

        private UserCustomFieldKey<TFieldScheme> BuildUserCustomFieldKey<TFieldScheme>(string fieldName, User.Contract.FieldKey[] fields)
        {
            return new UserCustomFieldKey<TFieldScheme>(fieldName, fieldIdIndex++)
            {
                Fields = fields
            };
        }

        private int fieldOptionIdIndex = 0;
        private ISelectOption BuildOption(string name, bool disabled = false)
        {
            return new SelectOption
            {
                Id = fieldOptionIdIndex++,
                Value = name,
                Disabled = disabled
            };
        }

        public void Dispose()
        {
            testContext.Dispose();
        }
    }
}
