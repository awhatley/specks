using System;
using System.Linq.Expressions;

namespace Specks.Comparisons
{
    /// <summary>
    /// A specification that is satisfied if the candidate object is of a specified type.
    /// </summary>
    internal class TypeIs<T> : Specification<T>
    {
        private Type Value { get; set; }

        public TypeIs(Type type)
        {
            Value = type;
        }

        protected override Expression<Func<T, bool>> BuildCriteria()
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var typeIs = Expression.TypeIs(parameter, Value);

            return Expression.Lambda<Func<T, bool>>(typeIs, parameter);
        }
    }
}