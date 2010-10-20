using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Comparisons;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests
{
    [TestFixture]
    public class FluentConstructionTestFixture
    {
        [Test]
        public void Where_CreatesNewInstance()
        {
            var spec = Specify<int>
                .Where<IntegerEqualToZero>();

            Assert.That(spec, Is.TypeOf<IntegerEqualToZero>());
        }

        [Test]
        public void Where_ReturnsProposition()
        {
            var criteria = (Expression<Func<int, bool>>)(x => x > 0);
            var spec = Specify<int>.Where(criteria);

            Assert.That(spec, Is.TypeOf<Proposition<int>>());
            Assert.That(spec, Has.Property("Criteria").EqualTo(criteria));
        }

        [Test]
        public void With_ReturnsConversion()
        {
            var inner = Specify.EqualTo(DayOfWeek.Monday);
            var converter = (Expression<Func<DateTime, DayOfWeek>>)(d => d.DayOfWeek);
            var spec = Specify<DateTime>.Where(converter, inner);

            Assert.That(spec, Is.TypeOf<Conversion<DateTime, DayOfWeek>>());
            Assert.That(spec, Has.Property("Inner").EqualTo(inner));
            Assert.That(spec, Has.Property("Converter").EqualTo(converter));
        }
        [Test]
        public void OfType_Fluent()
        {
            var spec = Specify<Attribute>.OfType<SerializableAttribute>();

            Assert.That(spec, Is.TypeOf<TypeIs<Attribute>>());
            Assert.That(spec, Has.Property("Value").EqualTo(typeof(SerializableAttribute)));
        }
    }
}