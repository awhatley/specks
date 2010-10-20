using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class ConversionTestFixture
    {
        [Test]
        public void ConvertsCriteria()
        {
            var spec = new IntegerPredicate(i => i == 0);
            var converter = (Expression<Func<double, int>>)(d => Math.Sign(d));
            var criteria = spec.Criteria;
            var convertedSpec = new Conversion<double, int>(spec, converter);
            var convertedCriteria = convertedSpec.Criteria;

            Assert.That(convertedCriteria.Body, Is.AssignableTo<BinaryExpression>());

            var binary = (BinaryExpression)convertedCriteria.Body;

            Assert.That(binary.NodeType, Is.EqualTo(ExpressionType.Equal));
            Assert.That(binary.Left, Is.EqualTo(converter.Body));
            Assert.That(binary.Right, Is.EqualTo(((BinaryExpression)criteria.Body).Right));
            Assert.That(binary.Method, Is.Null);
            Assert.That(binary.IsLifted, Is.False);
            Assert.That(binary.IsLiftedToNull, Is.False);
            Assert.That(binary.Conversion, Is.Null);

            ExpressionWriter.Write(convertedCriteria);
        }

        [Test]
        public void SelectsConversion()
        {
            var spec = new IntegerGreaterThanZero();
            var converter = (Expression<Func<double, int>>)(d => Math.Sign(d));
            var convertedSpec = new Conversion<double, int>(spec, converter);

            Assert.That(convertedSpec.IsSatisfiedBy(1), Is.True);
            Assert.That(convertedSpec.IsSatisfiedBy(0), Is.False);
            Assert.That(convertedSpec.IsSatisfiedBy(-1), Is.False);
        }
    }
}