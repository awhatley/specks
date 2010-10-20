using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class PropositionTestFixture
    {
        [Test]
        public void Criteria()
        {
            var spec = new IntegerPredicate(i => i == 0);
            var criteria = spec.Criteria;
            
            var proposition = new Proposition<int>(criteria);
            var proposedCriteria = proposition.Criteria;

            Assert.That(proposedCriteria.Body, Is.AssignableTo<BinaryExpression>());

            var binary = (BinaryExpression)proposedCriteria.Body;

            Assert.That(binary.NodeType, Is.EqualTo(ExpressionType.Equal));
            Assert.That(binary.Method, Is.Null);
            Assert.That(binary.IsLifted, Is.False);
            Assert.That(binary.IsLiftedToNull, Is.False);
            Assert.That(binary.Conversion, Is.Null);
            Assert.That(binary.Left, Is.AssignableTo<ParameterExpression>());
            Assert.That(binary.Right, Is.TypeOf<ConstantExpression>().With.Property("Value").EqualTo(0));

            ExpressionWriter.Write(proposedCriteria);
        }

        [Test]
        public void Selection()
        {
            var criteria = (Expression<Func<int, bool>>)(x => x > 0);
            var negatedSpec = new Proposition<int>(criteria);

            Assert.That(negatedSpec.IsSatisfiedBy(1), Is.True);
            Assert.That(negatedSpec.IsSatisfiedBy(0), Is.False);
            Assert.That(negatedSpec.IsSatisfiedBy(-1), Is.False);
        }
    }
}