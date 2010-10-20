using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class ExistentialQuantificationTestFixture
    {
        [Test]
        public void ExistentiallyQuantifiesCriteria()
        {
            var spec = new IntegerPredicate(i => i == 0);
            var criteria = spec.Criteria;

            var anySpec = new ExistentialQuantification<int>(spec);
            var anyCriteria = anySpec.Criteria;

            Assert.That(anyCriteria.Body, Is.AssignableTo<MethodCallExpression>());

            var call = (MethodCallExpression)anyCriteria.Body;

            Assert.That(call.NodeType, Is.EqualTo(ExpressionType.Call));
            Assert.That(call.Method, Is.EqualTo(typeof(Enumerable).GetMethods().Where(m => m.Name == "Any").ToArray()[1].MakeGenericMethod(new[] { typeof(int) })));
            Assert.That(call.Object, Is.Null);
            Assert.That(call.Arguments, Has.Count.EqualTo(2));
            Assert.That(call.Arguments[0], Is.AssignableTo<ParameterExpression>());
            Assert.That(call.Arguments[1], Is.EqualTo(criteria));

            ExpressionWriter.Write(anyCriteria);
        }

        [Test]
        public void SelectsExistentialQuantification()
        {
            var spec = new IntegerGreaterThanZero().ForAny();

            Assert.IsTrue(spec.IsSatisfiedBy(new[] { -1, -2, -3, -4, 0, 1, 2 }));
            Assert.IsFalse(spec.IsSatisfiedBy(new[] { -1, -2, -3, -4, 0 }));
        }
    }
}