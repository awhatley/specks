using System;
using System.Linq.Expressions;

using Specks.Expressions;

namespace Specks.Logic
{
    /// <summary>
    /// A composite specification that is satisfied only if both components are satisfied.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
    internal class Conjunction<T> : Specification<T>
    {
        private Specification<T> Left { get; set; }
        private Specification<T> Right { get; set; }

        public Conjunction(Specification<T> left, Specification<T> right)
        {
            Left = left;
            Right = right;
        }

        protected override Expression<Func<T, bool>> BuildCriteria()
        {
            var left = Left.Criteria;
            var right = Right.Criteria;

            var parameter = left.Parameters[0];
            var rightBody = ExpressionReplacer.Replace(right.Body, right.Parameters[0], parameter);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, rightBody), parameter);
        }
    }
}