using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class UniqueQuantificationTestFixture
    {
        [Test]
        public void UniquelyQuantifiesCriteria()
        {
            var spec = new IntegerPredicate(i => i == 0);
            var criteria = spec.Criteria;

            var singleSpec = new UniqueQuantification<int>(spec);
            var singleCriteria = singleSpec.Criteria;

            Assert.That(singleCriteria.Body, Is.AssignableTo<BinaryExpression>());

            var binary = (BinaryExpression)singleCriteria.Body;

            Assert.That(binary.Conversion, Is.Null);
            Assert.That(binary.IsLifted, Is.False);
            Assert.That(binary.IsLiftedToNull, Is.False);
            Assert.That(binary.Method, Is.Null);
            Assert.That(binary.NodeType, Is.EqualTo(ExpressionType.Equal));
            Assert.That(binary.Type, Is.EqualTo(typeof(bool)));

            Assert.That(binary.Left, Is.AssignableTo<MethodCallExpression>());
            Assert.That(binary.Right, Is.TypeOf<ConstantExpression>());

            var call = (MethodCallExpression)binary.Left;
            var constant = (ConstantExpression)binary.Right;

            Assert.That(call.NodeType, Is.EqualTo(ExpressionType.Call));
            Assert.That(call.Method, Is.EqualTo(typeof(Enumerable).GetMethods().Where(m => m.Name == "Count").ToArray()[1].MakeGenericMethod(new[] { typeof(int) })));
            Assert.That(call.Object, Is.Null);
            Assert.That(call.Arguments, Has.Count.EqualTo(2));
            Assert.That(call.Arguments[0], Is.AssignableTo<ParameterExpression>());
            Assert.That(call.Arguments[1], Is.EqualTo(criteria));

            Assert.That(constant.Value, Is.EqualTo(1));

            ExpressionWriter.Write(singleCriteria);
        }

        [Test]
        public void SelectsUniqueQuantification()
        {
            var spec = new IntegerGreaterThanZero().ForOne();

            Assert.IsTrue(spec.IsSatisfiedBy(new[] { -1, -2, -3, -4, 0, 1 }));
            Assert.IsFalse(spec.IsSatisfiedBy(new[] { -1, -2, -3, -4, 0 }));
            Assert.IsFalse(spec.IsSatisfiedBy(new[] { -1, -2, -3, -4, 0, 1, 2 }));
        }
    }
}