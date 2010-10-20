using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class ContradictionTestFixture
    {
        [Test]
        public void Criteria()
        {
            var contradiction = new Contradiction<int>();
            var contradictionCriteria = contradiction.Criteria;

            Assert.That(contradictionCriteria.Body, Is.AssignableTo<ConstantExpression>());

            var constant = (ConstantExpression)contradictionCriteria.Body;

            Assert.That(constant.NodeType, Is.EqualTo(ExpressionType.Constant));
            Assert.That(constant.Value, Is.False);

            ExpressionWriter.Write(contradictionCriteria);
        }

        [Test]
        public void Selection()
        {
            var contradictionSpec = new Contradiction<int>();

            Assert.That(contradictionSpec.IsSatisfiedBy(Int32.MinValue), Is.False);
            Assert.That(contradictionSpec.IsSatisfiedBy(1), Is.False);
            Assert.That(contradictionSpec.IsSatisfiedBy(0), Is.False);
            Assert.That(contradictionSpec.IsSatisfiedBy(-1), Is.False);
            Assert.That(contradictionSpec.IsSatisfiedBy(Int32.MaxValue), Is.False);
        }
    }
}