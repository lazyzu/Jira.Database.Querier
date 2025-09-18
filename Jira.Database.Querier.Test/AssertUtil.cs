using Equ;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lazyzu.Jira.Database.Querier.Test
{
    public static class AssertUtil
    {
        public static void MemberwisePropertiesEqual<T>(object actual, object expected)
        {
            var isMemberwisePropertiesEqual = MemberwiseEqualityComparer<T>.ByProperties.Equals((T)actual, (T)expected);
            Assert.That(isMemberwisePropertiesEqual);
        }

        public static void MemberwisePropertiesNotEqual<T>(object actual, object expected)
        {
            var isMemberwisePropertiesEqual = MemberwiseEqualityComparer<T>.ByProperties.Equals((T)actual, (T)expected);
            Assert.That(isMemberwisePropertiesEqual, Is.False);
        }

        public static void EquivalentToAndMemberwisePropertiesEqual<T>(IEnumerable<object> actual, IEnumerable<object> expected, Func<T, IComparable> keySelector)
        {
            Assert.That(actual, Is.EquivalentTo(expected));

            if (actual.Any() && expected.Any())
            {
                var actualFirst = actual.Cast<T>().OrderBy(keySelector).First();
                var expectedFirst = expected.Cast<T>().OrderBy(keySelector).First();

                MemberwisePropertiesEqual<T>(actualFirst, expectedFirst);
            }
        }

        public static void EquivalentToAndMemberwisePropertiesEqual<T>(IEnumerable<object> actual, IEnumerable<object> expected, Func<T, IComparable> keySelector, Action<T,T> equalAssert)
        {
            Assert.That(actual, Is.EquivalentTo(expected));

            if (actual.Any() && expected.Any())
            {
                var actualFirst = actual.Cast<T>().OrderBy(keySelector).First();
                var expectedFirst = expected.Cast<T>().OrderBy(keySelector).First();

                equalAssert(actualFirst, expectedFirst);
            }
        }

        public static void EquivalentToAndDefaultEqual<T>(IEnumerable<object> actual, IEnumerable<object> expected, Func<T, IComparable> keySelector)
        {
            Assert.That(actual, Is.EquivalentTo(expected));

            var actualFirst = actual.Cast<T>().OrderBy(keySelector).First();
            var expectedFirst = expected.Cast<T>().OrderBy(keySelector).First();

            Assert.That(actualFirst, Is.EqualTo(expectedFirst));
        }

    }
}
