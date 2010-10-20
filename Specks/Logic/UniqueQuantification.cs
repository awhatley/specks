using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Specks.Logic
{
    /// <summary>
    /// A specification that is satisfied if the inner specification is satisfied by only one
    /// element in a candidate collection.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the inner specification.</typeparam>
    internal class UniqueQuantification<T> : Specification<IEnumerable<T>>
    {
        private static readonly MethodInfo _countMethod = typeof(Enumerable)
            .GetMethods()
            .Where(m => m.Name == "Count")
            .ToArray()[1]
            .MakeGenericMethod(new[] { typeof(T) });

        private Specification<T> Inner { get; set; }

        public UniqueQuantification(Specification<T> inner)
        {
            Inner = inner;
        }

        protected override Expression<Func<IEnumerable<T>, bool>> BuildCriteria()
        {
            var param = Expression.Parameter(typeof(IEnumerable<T>), "x");
            var criteria = Inner.Criteria;
            var countCall = Expression.Call(_countMethod, param, criteria);
            var equalsOne = Expression.Equal(countCall, Expression.Constant(1, typeof(int)));

            return Expression.Lambda<Func<IEnumerable<T>, bool>>(equalsOne, param);
        }
    }
}