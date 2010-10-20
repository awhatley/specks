using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Comparisons;
using Specks.Expressions;

namespace Specks.Tests.Comparisons
{
    [TestFixture]
    public class BinaryComparisonTestFixture
    {
        [Test]
        public void BinaryComparison_Criteria()
        {
            var spec = new BinaryComparison<int>(5, Expression.Equal);
            var criteria = spec.Criteria;

            Assert.That(criteria.Body, Is.AssignableTo<BinaryExpression>());

            var binary = (BinaryExpression)criteria.Body;

            Assert.That(binary.NodeType, Is.EqualTo(ExpressionType.Equal));
            Assert.That(binary.Conversion, Is.Null);
            Assert.That(binary.Method, Is.Null);
            Assert.That(binary.IsLifted, Is.False);
            Assert.That(binary.IsLiftedToNull, Is.False);
            Assert.That(binary.Left, Is.AssignableTo<ParameterExpression>());
            Assert.That(binary.Right, Is.TypeOf<ConstantExpression>());

            var right = (ConstantExpression)binary.Right;
            
            Assert.That(right.Value, Is.EqualTo(5));

            ExpressionWriter.Write(criteria);
        }
    }
}