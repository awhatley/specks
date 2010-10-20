using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using Specks.Comparisons;
using Specks.Logic;

namespace Specks
{
    /// <summary>
    /// Provides a set of static methods for constructing specification instances.
    /// </summary>
    public static class Specify
    {
        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified value.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="value">The value to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> EqualTo<T>(T value)
        {
            return new BinaryComparison<T>(value, Expression.Equal);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified value.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="value">The value to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> NotEqualTo<T>(T value)
        {
            return new BinaryComparison<T>(value, Expression.NotEqual);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified value.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="value">The value to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> LessThan<T>(T value)
        {
            return new BinaryComparison<T>(value, Expression.LessThan);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified value.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="value">The value to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> LessThanOrEqualTo<T>(T value)
        {
            return new BinaryComparison<T>(value, Expression.LessThanOrEqual);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified value.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="value">The value to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> AtMost<T>(T value)
        {
            return new BinaryComparison<T>(value, Expression.LessThanOrEqual);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified value.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="value">The value to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> GreaterThan<T>(T value)
        {
            return new BinaryComparison<T>(value, Expression.GreaterThan);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified value.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="value">The value to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> GreaterThanOrEqualTo<T>(T value)
        {
            return new BinaryComparison<T>(value, Expression.GreaterThanOrEqual);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified value.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="value">The value to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> AtLeast<T>(T value)
        {
            return new BinaryComparison<T>(value, Expression.GreaterThanOrEqual);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified values.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="lower">The lower bound to use for the comparison.</param>
        /// <param name="upper">The upper bound to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> Between<T>(T lower, T upper)
        {
            return GreaterThan(lower).And(LessThan(upper));
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified values.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="lower">The lower bound to use for the comparison.</param>
        /// <param name="upper">The upper bound to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> InRange<T>(T lower, T upper)
        {
            return AtLeast(lower).And(AtMost(upper));
        }

        /// <summary>
        /// Constructs a specification comparing a candidate value to the specified values.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <param name="lower">The lower bound to use for the comparison.</param>
        /// <param name="upper">The upper bound to use for the comparison.</param>
        /// <returns>A specification comparing a candidate value to the specified value.</returns>
        public static Specification<T> Outside<T>(T lower, T upper)
        {
            return LessThan(lower).Or(GreaterThan(upper));
        }

        /// <summary>
        /// Constructs a specification comparing a boolean value to <b>true</b>.
        /// </summary>
        /// <returns>A specification comparing a boolean value to <b>true</b>.</returns>
        public static Specification<bool> IsTrue()
        {
            return new Proposition<bool>(x => x);
        }

        /// <summary>
        /// Constructs a specification comparing a boolean value to <b>false</b>.
        /// </summary>
        /// <returns>A specification comparing a boolean value to <b>false</b>.</returns>
        public static Specification<bool> IsFalse()
        {
            return new Proposition<bool>(x => x).IsFalse();
        }

        /// <summary>
        /// Constructs a specification comparing a candidate string value to the specified string.
        /// </summary>
        /// <param name="expected">The string to use for the comparison.</param>
        /// <returns>A specification comparing a string value to the specified string.</returns>
        public static Specification<string> StartsWith(string expected)
        {
            return new StartsWith(expected);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate string value to the specified string.
        /// </summary>
        /// <param name="expected">The string to use for the comparison.</param>
        /// <param name="comparisonType">A value indicating how the comparison should be performed.</param>
        /// <returns>A specification comparing a string value to the specified string.</returns>
        public static Specification<string> StartsWith(string expected, StringComparison comparisonType)
        {
            return new StartsWith(expected, comparisonType);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate string value to the specified string.
        /// </summary>
        /// <param name="expected">The string to use for the comparison.</param>
        /// <returns>A specification comparing a string value to the specified string.</returns>
        public static Specification<string> EndsWith(string expected)
        {
            return new EndsWith(expected);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate string value to the specified string.
        /// </summary>
        /// <param name="expected">The string to use for the comparison.</param>
        /// <param name="comparisonType">A value indicating how the comparison should be performed.</param>
        /// <returns>A specification comparing a string value to the specified string</returns>
        public static Specification<string> EndsWith(string expected, StringComparison comparisonType)
        {
            return new EndsWith(expected, comparisonType);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate string value to the specified string.
        /// </summary>
        /// <param name="expected">The string to use for the comparison.</param>
        /// <returns>A specification comparing a string value to the specified string.</returns>
        public static Specification<string> Contains(string expected)
        {
            return new Contains(expected);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate string value to the specified string.
        /// </summary>
        /// <param name="expected">The string to use for the comparison.</param>
        /// <param name="comparisonType">A value indicating how the comparison should be performed.</param>
        /// <returns>A specification comparing a string value to the specified string.</returns>
        public static Specification<string> Contains(string expected, StringComparison comparisonType)
        {
            return new Contains(expected, comparisonType);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate string value to the specified regular expression.
        /// </summary>
        /// <param name="expression">The expression to use for the comparison.</param>
        /// <returns>A specification comparing a string value to the specified regular expression.</returns>
        public static Specification<string> MatchesRegex(string expression)
        {
            return new MatchesRegex(expression);
        }

        /// <summary>
        /// Constructs a specification comparing a candidate string value to the specified regular expression.
        /// </summary>
        /// <param name="expression">The expression to use for the comparison.</param>
        /// <param name="options">A value indicating how the comparison should be performed.</param>
        /// <returns>A specification comparing a string value to the specified regular expression.</returns>
        public static Specification<string> MatchesRegex(string expression, RegexOptions options)
        {
            return new MatchesRegex(expression, options);
        }

        /// <summary>
        /// Constructs a specification that every candidate satisifies.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <returns>A specification that every candidate satisifies.</returns>
        public static Specification<T> Any<T>()
        {
            return new Tautology<T>();
        }

        /// <summary>
        /// Constructs a specification that no candidate satisifies.
        /// </summary>
        /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
        /// <returns>A specification that no candidate satisifies.</returns>
        public static Specification<T> No<T>()
        {
            return new Contradiction<T>();
        }
    }

    /// <summary>
    /// Provides a set of static methods for constructing specification instances 
    /// for a given candidate object type.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
    public static class Specify<T>
    {
        /// <summary>
        /// Constructs a specification using the indicated specification type as a root. Typically, this is
        /// used to begin the process of combining multiple specification types together.
        /// </summary>
        /// <typeparam name="TSpecification">The type of specification to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="TSpecification"/>.</returns>
        public static Specification<T> Where<TSpecification>() where TSpecification : Specification<T>, new()
        {
            return new TSpecification();
        }

        /// <summary>
        /// Constructs an ad-hoc specification using the provided criteria expression.
        /// </summary>
        /// <param name="condition">The expression to use for the specification criteria.</param>
        /// <returns>An ad-hoc specification using the provided criteria expression</returns>
        public static Specification<T> Where(Expression<Func<T, bool>> condition)
        {
            return new Proposition<T>(condition);
        }

        /// <summary>
        /// Constructs a specification for evaluating properties or fields of a candidate object against
        /// strongly-typed criteria.
        /// </summary>
        /// <typeparam name="TValue">The candidate object type of the inner specification.</typeparam>
        /// <param name="expression">The expression used to retrieve the value of a field or property to test.</param>
        /// <param name="criteria">The inner specification to evaluate against the field or property value. Typically,
        /// this is done by calling a <see cref="Specify"/> method.</param>
        /// <returns>A new specification containing the inner specification.</returns>
        public static Specification<T> Where<TValue>(Expression<Func<T, TValue>> expression, Specification<TValue> criteria)
        {
            return new Conversion<T, TValue>(criteria, expression);
        }

        /// <summary>
        /// Constructs a specification for evaluating the type of a candidate object.
        /// </summary>
        /// <typeparam name="U">The candidate object type expected.</typeparam>
        /// <returns>A specification for evaluating the type of a candidate object</returns>
        public static Specification<T> OfType<U>()
        {
            return new TypeIs<T>(typeof(U));
        }
    }
}