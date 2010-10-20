using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class TautologyTestFixture
    {
        [Test]
        public void Criteria()
        {
            var tautology = new Tautology<int>();
            var tautologyCriteria = tautology.Criteria;

            Assert.That(tautologyCriteria.Body, Is.AssignableTo<ConstantExpression>());

            var constant = (ConstantExpression)tautologyCriteria.Body;

            Assert.That(constant.NodeType, Is.EqualTo(ExpressionType.Constant));
            Assert.That(constant.Value, Is.True);

            ExpressionWriter.Write(tautologyCriteria);
        }

        [Test]
        public void Selection()
        {
            var tautologySpec = new Tautology<int>();

            Assert.That(tautologySpec.IsSatisfiedBy(Int32.MinValue), Is.True);
            Assert.That(tautologySpec.IsSatisfiedBy(1), Is.True);
            Assert.That(tautologySpec.IsSatisfiedBy(0), Is.True);
            Assert.That(tautologySpec.IsSatisfiedBy(-1), Is.True);
            Assert.That(tautologySpec.IsSatisfiedBy(Int32.MaxValue), Is.True);
        }
    }
}