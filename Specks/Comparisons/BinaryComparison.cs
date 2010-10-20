using System;
using System.Linq.Expressions;

namespace Specks.Comparisons
{
    /// <summary>
    /// A specification that is satisfied by evaluating a candidate value against a specified binary operation.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
    internal class BinaryComparison<T> : Specification<T>
    {
        private T Value { get; set; }
        private Func<Expression, Expression, BinaryExpression> Operation { get; set; }

        public BinaryComparison(T value, Func<Expression, Expression, BinaryExpression> operation)
        {
            Value = value;
            Operation = operation;
        }

        protected override Expression<Func<T, bool>> BuildCriteria()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var constant = Expression.Constant(Value, typeof(T));

            return Expression.Lambda<Func<T, bool>>(Operation(parameter, constant), parameter);
        }
    }
}