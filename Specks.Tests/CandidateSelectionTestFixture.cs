using System.Linq;

using NUnit.Framework;

using Specks.Tests.Stubs;

namespace Specks.Tests
{
    [TestFixture]
    public class CandidateSelectionTestFixture
    {
        [Test]
        public void IsSatisfiedBy_Uses_Criteria()
        {
            var spec = new IntegerGreaterThanZero();
            
            Assert.That(spec.IsSatisfiedBy(1), Is.True);
            Assert.That(spec.IsSatisfiedBy(0), Is.False);
            Assert.That(spec.IsSatisfiedBy(-1), Is.False);
        }

        [Test]
        public void Filter_IEnumerable_Uses_Criteria()
        {
            var spec = new IntegerGreaterThanZero();
            var candidates = (new[] { -1, 0, 1, 2, 3 }).AsEnumerable();

            var enumerable = spec.Filter(candidates);
            foreach(var i in enumerable)
            {
                Assert.That(spec.IsSatisfiedBy(i));
            }
        }

        [Test]
        public void Filter_IQueryable_Uses_Criteria()
        {
            var spec = new IntegerGreaterThanZero();
            var candidates = (new[] { -1, 0, 1, 2, 3 }).AsQueryable();

            var queryable = spec.Filter(candidates);
            foreach(var i in queryable)
            {
                Assert.That(spec.IsSatisfiedBy(i));
            }
        }
    }
}