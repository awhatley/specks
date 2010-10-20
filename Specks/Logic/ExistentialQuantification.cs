using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Specks.Logic
{
    /// <summary>
    /// A specification that is satisfied if the inner specification is satisfied by any
    /// elements in a candidate collection.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the inner specification.</typeparam>
    internal class ExistentialQuantification<T> : Specification<IEnumerable<T>>
    {
        private static readonly MethodInfo _anyMethod = typeof(Enumerable)
            .GetMethods()
            .Where(m => m.Name == "Any")
            .ToArray()[1]
            .MakeGenericMethod(new[] { typeof(T) });

        private Specification<T> Inner { get; set; }

        public ExistentialQuantification(Specification<T> inner)
        {
            Inner = inner;
        }

        protected override Expression<Func<IEnumerable<T>, bool>> BuildCriteria()
        {
            var param = Expression.Parameter(typeof(IEnumerable<T>), "x");
            var criteria = Inner.Criteria;
            var allCall = Expression.Call(_anyMethod, param, criteria);

            return Expression.Lambda<Func<IEnumerable<T>, bool>>(allCall, param);
        }
    }
}