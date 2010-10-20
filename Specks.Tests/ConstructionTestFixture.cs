using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Specks.Comparisons;
using Specks.Logic;

using Contains = Specks.Comparisons.Contains;

namespace Specks.Tests
{
    [TestFixture]
    public class ConstructionTestFixture
    {
        [Test]
        public void EqualTo_ReturnsEquality()
        {
            var spec = Specify.EqualTo(5);
            var expression = (Func<Expression, Expression, BinaryExpression>)Expression.Equal;

            Assert.That(spec, Is.TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Operation").EqualTo(expression));
        }

        [Test]
        public void NotEqualTo_ReturnsInequality()
        {
            var spec = Specify.NotEqualTo(5);
            var expression = (Func<Expression, Expression, BinaryExpression>)Expression.NotEqual;

            Assert.That(spec, Is.TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Operation").EqualTo(expression));
        }

        [Test]
        public void LessThan_ReturnsLessThan()
        {
            var spec = Specify.LessThan(5);
            var expression = (Func<Expression, Expression, BinaryExpression>)Expression.LessThan;

            Assert.That(spec, Is.TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Operation").EqualTo(expression));
        }

        [Test]
        public void LessThanOrEqualTo_ReturnsLessThanOrEqualTo()
        {
            var spec = Specify.LessThanOrEqualTo(5);
            var expression = (Func<Expression, Expression, BinaryExpression>)Expression.LessThanOrEqual;

            Assert.That(spec, Is.TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Operation").EqualTo(expression));
        }

        [Test]
        public void AtMost_ReturnsLessThanOrEqualTo()
        {
            var spec = Specify.AtMost(5);
            var expression = (Func<Expression, Expression, BinaryExpression>)Expression.LessThanOrEqual;

            Assert.That(spec, Is.TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Operation").EqualTo(expression));
        }

        [Test]
        public void GreaterThan_ReturnsGreaterThan()
        {
            var spec = Specify.GreaterThan(5);
            var expression = (Func<Expression, Expression, BinaryExpression>)Expression.GreaterThan;

            Assert.That(spec, Is.TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Operation").EqualTo(expression));
        }

        [Test]
        public void GreaterThanOrEqualTo_ReturnsGreaterThanOrEqualTo()
        {
            var spec = Specify.GreaterThanOrEqualTo(5);
            var expression = (Func<Expression, Expression, BinaryExpression>)Expression.GreaterThanOrEqual;

            Assert.That(spec, Is.TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Operation").EqualTo(expression));
        }

        [Test]
        public void AtLeast_ReturnsGreaterThanOrEqualTo()
        {
            var spec = Specify.AtLeast(5);
            var expression = (Func<Expression, Expression, BinaryExpression>)Expression.GreaterThanOrEqual;

            Assert.That(spec, Is.TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Operation").EqualTo(expression));
        }

        [Test]
        public void Between_ReturnsGreaterThanAndLessThan()
        {
            var spec = Specify.Between(5, 10);
            var greaterThanOrEqual = (Func<Expression, Expression, BinaryExpression>)Expression.GreaterThan;
            var lessThanOrEqual = (Func<Expression, Expression, BinaryExpression>)Expression.LessThan;

            Assert.That(spec, Is.TypeOf<Conjunction<int>>());
            Assert.That(spec, Has.Property("Left").TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Left").With.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Left").With.Property("Operation").EqualTo(greaterThanOrEqual));
            Assert.That(spec, Has.Property("Right").TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Right").With.Property("Value").EqualTo(10));
            Assert.That(spec, Has.Property("Right").With.Property("Operation").EqualTo(lessThanOrEqual));
        }

        [Test]
        public void InRange_ReturnsAtLeastAndAtMost()
        {
            var spec = Specify.InRange(5, 10);
            var greaterThanOrEqual = (Func<Expression, Expression, BinaryExpression>)Expression.GreaterThanOrEqual;
            var lessThanOrEqual = (Func<Expression, Expression, BinaryExpression>)Expression.LessThanOrEqual;

            Assert.That(spec, Is.TypeOf<Conjunction<int>>());
            Assert.That(spec, Has.Property("Left").TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Left").With.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Left").With.Property("Operation").EqualTo(greaterThanOrEqual));
            Assert.That(spec, Has.Property("Right").TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Right").With.Property("Value").EqualTo(10));
            Assert.That(spec, Has.Property("Right").With.Property("Operation").EqualTo(lessThanOrEqual));
        }

        [Test]
        public void Outside_ReturnsAtLeastAndAtMost()
        {
            var spec = Specify.Outside(5, 10);
            var greaterThan = (Func<Expression, Expression, BinaryExpression>)Expression.GreaterThan;
            var lessThan = (Func<Expression, Expression, BinaryExpression>)Expression.LessThan;

            Assert.That(spec, Is.TypeOf<Disjunction<int>>());
            Assert.That(spec, Has.Property("Left").TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Left").With.Property("Value").EqualTo(5));
            Assert.That(spec, Has.Property("Left").With.Property("Operation").EqualTo(lessThan));
            Assert.That(spec, Has.Property("Right").TypeOf<BinaryComparison<int>>());
            Assert.That(spec, Has.Property("Right").With.Property("Value").EqualTo(10));
            Assert.That(spec, Has.Property("Right").With.Property("Operation").EqualTo(greaterThan));
        }

        [Test]
        public void IsTrue_ReturnsProposition()
        {
            var spec = Specify.IsTrue();

            Assert.That(spec, Is.TypeOf<Proposition<bool>>());
            Assert.That(spec, Has.Property("Criteria").With.Property("Body").AssignableTo<ParameterExpression>());
        }

        [Test]
        public void IsFalse_ReturnsNegation()
        {
            var spec = Specify.IsFalse();

            Assert.That(spec, Is.TypeOf<Negation<bool>>());
            Assert.That(spec, Has.Property("Inner").TypeOf<Proposition<bool>>());
            Assert.That(spec, Has.Property("Inner").With.Property("Criteria").With.Property("Body").AssignableTo<ParameterExpression>());
        }

        [Test]
        public void StartsWith_ReturnsStartsWith()
        {
            var spec = Specify.StartsWith("abc");

            Assert.That(spec, Is.TypeOf<StartsWith>());
            Assert.That(spec, Has.Property("Value").EqualTo("abc"));
            Assert.That(spec, Has.Property("ComparisonType").EqualTo(StringComparison.InvariantCultureIgnoreCase));
        }

        [Test]
        public void StartsWithOptions_ReturnsStartsWith()
        {
            var spec = Specify.StartsWith("abc", StringComparison.Ordinal);

            Assert.That(spec, Is.TypeOf<StartsWith>());
            Assert.That(spec, Has.Property("Value").EqualTo("abc"));
            Assert.That(spec, Has.Property("ComparisonType").EqualTo(StringComparison.Ordinal));
        }

        [Test]
        public void EndsWith_ReturnsEndsWith()
        {
            var spec = Specify.EndsWith("abc");

            Assert.That(spec, Is.TypeOf<EndsWith>());
            Assert.That(spec, Has.Property("Value").EqualTo("abc"));
            Assert.That(spec, Has.Property("ComparisonType").EqualTo(StringComparison.InvariantCultureIgnoreCase));
        }

        [Test]
        public void EndsWithOptions_ReturnsEndsWith()
        {
            var spec = Specify.EndsWith("abc", StringComparison.Ordinal);

            Assert.That(spec, Is.TypeOf<EndsWith>());
            Assert.That(spec, Has.Property("Value").EqualTo("abc"));
            Assert.That(spec, Has.Property("ComparisonType").EqualTo(StringComparison.Ordinal));
        }

        [Test]
        public void Contains_ReturnsContains()
        {
            var spec = Specify.Contains("abc");

            Assert.That(spec, Is.TypeOf<Contains>());
            Assert.That(spec, Has.Property("Value").EqualTo("abc"));
            Assert.That(spec, Has.Property("ComparisonType").EqualTo(StringComparison.InvariantCultureIgnoreCase));
        }

        [Test]
        public void ContainsOptions_ReturnsContains()
        {
            var spec = Specify.Contains("abc", StringComparison.Ordinal);

            Assert.That(spec, Is.TypeOf<Contains>());
            Assert.That(spec, Has.Property("Value").EqualTo("abc"));
            Assert.That(spec, Has.Property("ComparisonType").EqualTo(StringComparison.Ordinal));
        }

        [Test]
        public void MatchesRegex_ReturnsMatchesRegex()
        {
            var spec = Specify.MatchesRegex("abc");

            Assert.That(spec, Is.TypeOf<MatchesRegex>());
            Assert.That(spec, Has.Property("Pattern").EqualTo("abc"));
            Assert.That(spec, Has.Property("Options").EqualTo(RegexOptions.None));
        }

        [Test]
        public void MatchesRegexOptions_ReturnsMatchesRegex()
        {
            var spec = Specify.MatchesRegex("abc", RegexOptions.Compiled);

            Assert.That(spec, Is.TypeOf<MatchesRegex>());
            Assert.That(spec, Has.Property("Pattern").EqualTo("abc"));
            Assert.That(spec, Has.Property("Options").EqualTo(RegexOptions.Compiled));
        }
    }
}