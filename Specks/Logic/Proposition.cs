using System;
using System.Linq.Expressions;

namespace Specks.Logic
{
    /// <summary>
    /// A specification that is satisfied using a specified proposition.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
    internal class Proposition<T> : Specification<T>
    {
        private Expression<Func<T, bool>> InnerCriteria { get; set; }
        
        public Proposition(Expression<Func<T, bool>> criteria)
        {
            InnerCriteria = criteria;
        }

        protected override Expression<Func<T, bool>> BuildCriteria()
        {
            return InnerCriteria;
        }
    }
}