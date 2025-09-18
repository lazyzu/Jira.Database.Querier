using lazyzu.Jira.Database.Querier.Issue.Fields;
using lazyzu.Jira.Database.Querier.Issue.Fields.Custom;
using lazyzu.Jira.Database.Querier.Project.Fields;
using NUnit.Framework;

namespace lazyzu.Jira.Database.Querier.Test
{
    public class EqualityTest
    {
        [Test]
        public void FullProjectComponentIdEquality()
        {
            var left = new FullProjectComponent
            {
                Id = 1
            };

            var right = new FullProjectComponent
            {
                Id = 1
            };

            Assert.That(left, Is.EqualTo(right));
        }

        [Test]
        public void MultipleSelectIdEquality()
        {
            var left = new ISelectOption[]
            {
                new SelectOption
                {
                    Id = 1,
                },
                new SelectOption
                {
                    Id = 2,
                },
            }.AsCollection();

            var right = new ISelectOption[]
            {
                new SelectOption
                {
                    Id = 2,
                },
                new SelectOption
                {
                    Id = 1,
                },
            }.AsCollection();

            Assert.That(left, Is.EqualTo(right));
        }

        [Test]
        public void CascadingSelectIdEquality()
        {
            var left = new ISelectOption[]
            {
                new SelectOption
                {
                    Id = 1,
                },
                new SelectOption
                {
                    Id = 2,
                },
            }.AsCascading();

            var right = new ISelectOption[]
            {
                new SelectOption
                {
                    Id = 1,
                },
                new SelectOption
                {
                    Id = 2,
                },
            }.AsCascading();

            Assert.That(left, Is.EqualTo(right));
        }

        [Test]
        public void CascadingSelectIdEquality_WrongOrder()
        {
            var left = new ISelectOption[]
            {
                new SelectOption
                {
                    Id = 1,
                },
                new SelectOption
                {
                    Id = 2,
                },
            }.AsCascading();

            var right = new ISelectOption[]
            {
                new SelectOption
                {
                    Id = 2,
                },
                new SelectOption
                {
                    Id = 1,
                },
            }.AsCascading();

            Assert.That(left, Is.Not.EqualTo(right));
        }

        [Test]
        public void IssueStatusCategory_MemberwiseEquality()
        {
            var left = new IssueStatusCategory()
            {
                Id = 1,
                Name = "left"
            };

            var right = new IssueStatusCategory()
            {
                Id = 1,
                Name = "right"
            };

            Assert.That(left, Is.EqualTo(right));   // Check By Id
            AssertUtil.MemberwisePropertiesNotEqual<IssueStatusCategory>(left, right); // All property
        }
    }
}
