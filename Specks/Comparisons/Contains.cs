using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Specks.Comparisons
{
    /// <summary>
    /// A specification that is satisfied if the candidate string contains a specified value.
    /// </summary>
    internal class Contains : Specification<string>
    {
        private static readonly MethodInfo _indexOfMethod = typeof(String)
            .GetMethod("IndexOf", new[] { typeof(string), typeof(StringComparison) });

        private string Value { get; set; }
        private StringComparison ComparisonType { get; set; }

        public Contains(string value) : 
            this(value, StringComparison.InvariantCultureIgnoreCase) { }

        public Contains(string value, StringComparison comparisonType)
        {
            Value = value;
            ComparisonType = comparisonType;
        }

        protected override Expression<Func<string, bool>> BuildCriteria()
        {
            var parameter = Expression.Parameter(typeof(string), "x");
            var value = Expression.Constant(Value, typeof(string));
            var comparisonType = Expression.Constant(ComparisonType, typeof(StringComparison));
            var indexOf = Expression.Call(parameter, _indexOfMethod, value, comparisonType);
            var zero = Expression.Constant(0, typeof(int));
            var indexFound = Expression.GreaterThanOrEqual(indexOf, zero);

            return Expression.Lambda<Func<string, bool>>(indexFound, parameter);
        }
    }
}