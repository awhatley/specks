using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Tests.Stubs;

namespace Specks.Tests
{
    [TestFixture]
    public class TypeConversionTestFixture
    {
        [Test]
        public void ImplicitConversionFromSpecification()
        {
            var spec = new IntegerGreaterThanZero();
            Expression<Func<int, bool>> expression = spec;

            Assert.That(expression, Is.SameAs(spec.Criteria));
        }

        [Test]
        public void ImplicitConversionToSpecification()
        {
            Expression<Func<int, bool>> expression = (x => x > 0);
            Specification<int> spec = expression;

            Assert.That(spec.Criteria, Is.SameAs(expression));
        }
    }
}