using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class ExclusiveDisjunctionTestFixture
    {
        [Test]
        public void DisjoinsCriteria()
        {
            var spec1 = new IntegerPredicate(i => i == 0);
            var spec2 = new IntegerPredicate(i => i > 0);

            var criteria1 = spec1.Criteria;
            var criteria2 = spec2.Criteria;

            var disjoinedSpec = new ExclusiveDisjunction<int>(spec1, spec2);
            var disjoinedCriteria = disjoinedSpec.Criteria;

            Assert.That(disjoinedCriteria.Body, Is.AssignableTo<BinaryExpression>());

            var binary = (BinaryExpression)disjoinedCriteria.Body;

            Assert.That(binary.NodeType, Is.EqualTo(ExpressionType.ExclusiveOr));
            Assert.That(binary.Conversion, Is.Null);
            Assert.That(binary.Left.ToString(), Is.EqualTo(criteria1.Body.ToString()));
            Assert.That(binary.Right.ToString(), Is.EqualTo(criteria2.Body.ToString()));
            Assert.That(binary.Method, Is.Null);
            Assert.That(binary.IsLifted, Is.False);
            Assert.That(binary.IsLiftedToNull, Is.False);

            ExpressionWriter.Write(disjoinedCriteria);
        }

        [Test]
        public void SelectsExclusiveDisjunction()
        {
            var spec1 = new IntegerPredicate(i => i > 1);
            var spec2 = new IntegerPredicate(i => i > 0);
            var disjoinedSpec = new ExclusiveDisjunction<int>(spec1, spec2);

            Assert.That(disjoinedSpec.IsSatisfiedBy(2), Is.False);
            Assert.That(disjoinedSpec.IsSatisfiedBy(1), Is.True);
            Assert.That(disjoinedSpec.IsSatisfiedBy(-1), Is.False);
        }
    }
}