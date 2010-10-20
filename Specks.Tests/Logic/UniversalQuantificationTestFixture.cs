using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests.Logic
{
    [TestFixture]
    public class UniversalQuantificationTestFixture
    {
        [Test]
        public void UniversallyQuantifiesCriteria()
        {
            var spec = new IntegerPredicate(i => i == 0);
            var criteria = spec.Criteria;

            var allSpec = new UniversalQuantification<int>(spec);
            var allCriteria = allSpec.Criteria;

            Assert.That(allCriteria.Body, Is.AssignableTo<MethodCallExpression>());

            var call = (MethodCallExpression)allCriteria.Body;

            Assert.That(call.NodeType, Is.EqualTo(ExpressionType.Call));
            Assert.That(call.Method, Is.EqualTo(typeof(Enumerable).GetMethod("All").MakeGenericMethod(new[] { typeof(int) })));
            Assert.That(call.Object, Is.Null);
            Assert.That(call.Arguments, Has.Count.EqualTo(2));
            Assert.That(call.Arguments[0], Is.AssignableTo<ParameterExpression>());
            Assert.That(call.Arguments[1], Is.EqualTo(criteria));

            ExpressionWriter.Write(allCriteria);
        }

        [Test]
        public void SelectsUniversalQuantification()
        {
            var spec = new IntegerGreaterThanZero().ForAll();

            Assert.IsTrue(spec.IsSatisfiedBy(new[] { 1, 1, 2, 3, 4, 5, 6 }));
            Assert.IsFalse(spec.IsSatisfiedBy(new[] { 3, 32, 589, 0, 11 }));
        }
    }
}