using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Comparisons;
using Specks.Expressions;

namespace Specks.Tests.Comparisons
{
    [TestFixture]
    public class EndsWithTestFixture
    {
        [Test]
        public void EndsWithCriteria()
        {
            var spec = new EndsWith("value", StringComparison.Ordinal);
            var criteria = spec.Criteria;

            Assert.That(criteria.Body, Is.AssignableTo<MethodCallExpression>());

            var call = (MethodCallExpression)criteria.Body;

            Assert.That(call.Arguments, Has.Count.EqualTo(2));
            Assert.That(call.Arguments[0], Is.TypeOf<ConstantExpression>());
            Assert.That(call.Arguments[1], Is.TypeOf<ConstantExpression>());
            Assert.That(call.Method, Is.EqualTo(typeof(String).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) })));
            Assert.That(call.Object, Is.AssignableTo<ParameterExpression>());

            var arg0 = (ConstantExpression)call.Arguments[0];
            Assert.That(arg0.Value, Is.EqualTo("value"));

            var arg1 = (ConstantExpression)call.Arguments[1];
            Assert.That(arg1.Value, Is.EqualTo(StringComparison.Ordinal));

            ExpressionWriter.Write(criteria);
        }

        [Test]
        public void SelectsEndsWith()
        {
            var spec = new EndsWith("value");

            Assert.That(spec.IsSatisfiedBy("value"), Is.True);
            Assert.That(spec.IsSatisfiedBy("ValuE"), Is.True);
            Assert.That(spec.IsSatisfiedBy("avalue"), Is.True);
            Assert.That(spec.IsSatisfiedBy("valuea"), Is.False);
            Assert.That(spec.IsSatisfiedBy("val"), Is.False);
            Assert.That(spec.IsSatisfiedBy(String.Empty), Is.False);
            //Assert.That(spec.IsSatisfiedBy(null), Is.False);
        }
    }
}