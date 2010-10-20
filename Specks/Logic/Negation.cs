using System;
using System.Linq.Expressions;

namespace Specks.Logic
{
    /// <summary>
    /// A specification that is satisfied only if the inner specification is not.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
    internal class Negation<T> : Specification<T>
    {
        private Specification<T> Inner { get; set; }

        public Negation(Specification<T> inner)
        {
            Inner = inner;
        }

        protected override Expression<Func<T, bool>> BuildCriteria()
        {
            var criteria = Inner.Criteria;
            return Expression.Lambda<Func<T, bool>>(Expression.Not(criteria.Body), criteria.Parameters);
        }
    }
}