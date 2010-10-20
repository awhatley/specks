using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class ConjunctionTestFixture
    {
        [Test]
        public void ConjoinsCriteria()
        {
            var spec1 = new IntegerPredicate(i => i == 0);
            var spec2 = new IntegerPredicate(i => i > 0);

            var criteria1 = spec1.Criteria;
            var criteria2 = spec2.Criteria;

            var conjoinedSpec = new Conjunction<int>(spec1, spec2);
            var conjoinedCriteria = conjoinedSpec.Criteria;

            Assert.That(conjoinedCriteria.Body, Is.AssignableTo<BinaryExpression>());

            var binary = (BinaryExpression)conjoinedCriteria.Body;

            Assert.That(binary.NodeType, Is.EqualTo(ExpressionType.AndAlso));
            Assert.That(binary.Conversion, Is.Null);
            Assert.That(binary.Left.ToString(), Is.EqualTo(criteria1.Body.ToString()));
            Assert.That(binary.Right.ToString(), Is.EqualTo(criteria2.Body.ToString()));
            Assert.That(binary.Method, Is.Null);
            Assert.That(binary.IsLifted, Is.False);
            Assert.That(binary.IsLiftedToNull, Is.False);

            ExpressionWriter.Write(conjoinedCriteria);
        }

        [Test]
        public void SelectsConjunction()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerPredicate(i => i > -1);
            var conjoinedSpec = new Conjunction<int>(spec1, spec2);

            Assert.That(conjoinedSpec.IsSatisfiedBy(1), Is.True);
            Assert.That(conjoinedSpec.IsSatisfiedBy(0), Is.False);
            Assert.That(conjoinedSpec.IsSatisfiedBy(-1), Is.False);
        }
    }
}