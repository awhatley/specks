using System;
using System.Linq.Expressions;

namespace Specks.Logic
{
    /// <summary>
    /// A specification that is never satisfied.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
    internal class Contradiction<T> : Specification<T>
    {
        protected override Expression<Func<T, bool>> BuildCriteria()
        {
            return x => false;
        }
    }
}