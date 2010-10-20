using System.Linq;

using NUnit.Framework;

using Specks.Tests.Stubs;

namespace Specks.Tests
{
    [TestFixture]
    public class ExtensionsTestFixture
    {
        [Test]
        public void Matching_IEnumerable_Uses_Specification()
        {
            var spec = new IntegerGreaterThanZero();
            var candidates = (new[] { -1, 0, 1, 2, 3 }).AsEnumerable();

            var enumerable = candidates.Matching(spec);
            foreach(var i in enumerable)
            {
                Assert.That(spec.IsSatisfiedBy(i));
            }
        }

        [Test]
        public void Matching_IQueryable_Uses_Specification()
        {
            var spec = new IntegerGreaterThanZero();
            var candidates = (new[] { -1, 0, 1, 2, 3 }).AsQueryable();

            var queryable = candidates.Matching(spec);
            foreach(var i in queryable)
            {
                Assert.That(spec.IsSatisfiedBy(i));
            }
        }
    }
}