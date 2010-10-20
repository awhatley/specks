using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;

using Contains = Specks.Comparisons.Contains;

namespace Specks.Tests.Comparisons
{
    [TestFixture]
    public class ContainsTestFixture
    {
        [Test]
        public void ContainsCriteria()
        {
            var spec = new Contains("value", StringComparison.Ordinal);
            var criteria = spec.Criteria;

            Assert.That(criteria.Body, Is.AssignableTo<BinaryExpression>());

            var binary = (BinaryExpression)criteria.Body;

            Assert.That(binary.NodeType, Is.EqualTo(ExpressionType.GreaterThanOrEqual));
            Assert.That(binary.Conversion, Is.Null);
            Assert.That(binary.Method, Is.Null);
            Assert.That(binary.IsLifted, Is.False);
            Assert.That(binary.IsLiftedToNull, Is.False);

            Assert.That(binary.Left, Is.AssignableTo<MethodCallExpression>());
            Assert.That(binary.Right, Is.TypeOf<ConstantExpression>());

            var left = (MethodCallExpression)binary.Left;

            Assert.That(left.Arguments, Has.Count.EqualTo(2));
            Assert.That(left.Arguments[0], Is.TypeOf<ConstantExpression>());
            Assert.That(left.Arguments[1], Is.TypeOf<ConstantExpression>());
            Assert.That(left.Method, Is.EqualTo(typeof(String).GetMethod("IndexOf", new[] { typeof(string), typeof(StringComparison) })));
            Assert.That(left.Object, Is.AssignableTo<ParameterExpression>());
            
            var arg0 = (ConstantExpression)left.Arguments[0];
            Assert.That(arg0.Value, Is.EqualTo("value"));

            var arg1 = (ConstantExpression)left.Arguments[1];
            Assert.That(arg1.Value, Is.EqualTo(StringComparison.Ordinal));

            var right = (ConstantExpression)binary.Right;
            Assert.That(right.Value, Is.EqualTo(0));

            ExpressionWriter.Write(criteria);
        }

        [Test]
        public void SelectsContains()
        {
            var spec = new Contains("value");

            Assert.That(spec.IsSatisfiedBy("value"), Is.True);
            Assert.That(spec.IsSatisfiedBy("ValuE"), Is.True);
            Assert.That(spec.IsSatisfiedBy("avalue"), Is.True);
            Assert.That(spec.IsSatisfiedBy("valuea"), Is.True);
            Assert.That(spec.IsSatisfiedBy("val"), Is.False);
            Assert.That(spec.IsSatisfiedBy(String.Empty), Is.False);
            //Assert.That(spec.IsSatisfiedBy(null), Is.False);
        }
    }
}