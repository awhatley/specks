using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Specks.Comparisons
{
    /// <summary>
    /// A specification that is satisfied if the candidate string starts with a specified value.
    /// </summary>
    internal class StartsWith : Specification<string>
    {
        private static readonly MethodInfo _startsWithMethod = typeof(String)
            .GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });

        private string Value { get; set; }
        private StringComparison ComparisonType { get; set; }

        public StartsWith(string value) :
            this(value, StringComparison.InvariantCultureIgnoreCase) { }

        public StartsWith(string value, StringComparison comparisonType)
        {
            Value = value;
            ComparisonType = comparisonType;
        }

        protected override Expression<Func<string, bool>> BuildCriteria()
        {
            var parameter = Expression.Parameter(typeof(string), "x");
            var value = Expression.Constant(Value, typeof(string));
            var comparisonType = Expression.Constant(ComparisonType, typeof(StringComparison));
            var startsWith = Expression.Call(parameter, _startsWithMethod, value, comparisonType);

            return Expression.Lambda<Func<string, bool>>(startsWith, parameter);
        }
    }
}