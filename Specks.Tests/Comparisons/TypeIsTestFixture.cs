using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Comparisons;
using Specks.Expressions;

namespace Specks.Tests.Comparisons
{
    [TestFixture]
    public class TypeIsTestFixture
    {
        [Test]
        public void TypeIsCriteria()
        {
            var spec = new TypeIs<Attribute>(typeof(FlagsAttribute));
            var criteria = spec.Criteria;

            Assert.That(criteria.Body, Is.TypeOf<TypeBinaryExpression>());

            var binary = (TypeBinaryExpression)criteria.Body;

            Assert.That(binary.TypeOperand, Is.EqualTo(typeof(FlagsAttribute)));
            Assert.That(binary.Expression, Is.AssignableTo<ParameterExpression>());

            ExpressionWriter.Write(criteria);
        }

        [Test]
        public void SelectsTypeIs()
        {
            var spec = new TypeIs<Attribute>(typeof(FlagsAttribute));

            Assert.That(spec.IsSatisfiedBy(new FlagsAttribute()), Is.True);
            Assert.That(spec.IsSatisfiedBy(new ExplicitAttribute()), Is.False);
        }
    }
}