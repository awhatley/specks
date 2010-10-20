using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Specks.Comparisons;
using Specks.Expressions;

namespace Specks.Tests.Comparisons
{
    [TestFixture]
    public class MatchesRegexTestFixture
    {
        [Test]
        public void MatchesRegexCriteria()
        {
            var spec = new MatchesRegex("[a-z][0-9].+", RegexOptions.Compiled);
            var criteria = spec.Criteria;

            Assert.That(criteria.Body, Is.AssignableTo<MethodCallExpression>());

            var call = (MethodCallExpression)criteria.Body;

            Assert.That(call.Arguments, Has.Count.EqualTo(3));
            Assert.That(call.Arguments[0], Is.AssignableTo<ParameterExpression>());
            Assert.That(call.Arguments[1], Is.TypeOf<ConstantExpression>());
            Assert.That(call.Arguments[2], Is.TypeOf<ConstantExpression>());
            Assert.That(call.Method, Is.EqualTo(typeof(Regex).GetMethod("IsMatch", new[] { typeof(string), typeof(string), typeof(RegexOptions) })));
            Assert.That(call.Object, Is.Null);

            var arg1 = (ConstantExpression)call.Arguments[1];
            Assert.That(arg1.Value, Is.EqualTo("[a-z][0-9].+"));
            
            var arg2 = (ConstantExpression)call.Arguments[2];
            Assert.That(arg2.Value, Is.EqualTo(RegexOptions.Compiled));

            ExpressionWriter.Write(criteria);
        }

        [Test]
        public void SelectsMatchesRegex()
        {
            var spec = new MatchesRegex("[a-z][0-9].+");

            Assert.That(spec.IsSatisfiedBy("a1"), Is.False);
            Assert.That(spec.IsSatisfiedBy("a11"), Is.True);
            Assert.That(spec.IsSatisfiedBy("b2a"), Is.True);
            Assert.That(spec.IsSatisfiedBy("b3ds145a"), Is.True);
            Assert.That(spec.IsSatisfiedBy("abcd"), Is.False);
            Assert.That(spec.IsSatisfiedBy("012354"), Is.False);
            Assert.That(spec.IsSatisfiedBy(String.Empty), Is.False);
            //Assert.That(spec.IsSatisfiedBy(null), Is.False);
        }
    }
}