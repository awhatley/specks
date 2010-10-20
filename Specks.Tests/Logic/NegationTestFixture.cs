using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class NegationTestFixture
    {
        [Test]
        public void NegatesCriteria()
        {
            var spec = new IntegerPredicate(i => i == 0);
            var criteria = spec.Criteria;
            var negatedSpec = new Negation<int>(spec);
            var negatedCriteria = negatedSpec.Criteria;

            Assert.That(negatedCriteria.Body, Is.TypeOf<UnaryExpression>());
            
            var unary = (UnaryExpression)negatedCriteria.Body;

            Assert.That(unary.NodeType, Is.EqualTo(ExpressionType.Not));
            Assert.That(unary.Operand, Is.EqualTo(criteria.Body));
            Assert.That(unary.Method, Is.Null);
            Assert.That(unary.IsLifted, Is.False);
            Assert.That(unary.IsLiftedToNull, Is.False);

            ExpressionWriter.Write(negatedCriteria);
        }

        [Test]
        public void SelectsInverse()
        {
            var spec = new IntegerGreaterThanZero();
            var negatedSpec = new Negation<int>(spec);

            Assert.That(negatedSpec.IsSatisfiedBy(1), Is.False);
            Assert.That(negatedSpec.IsSatisfiedBy(0), Is.True);
            Assert.That(negatedSpec.IsSatisfiedBy(-1), Is.True);
        }
    }
}