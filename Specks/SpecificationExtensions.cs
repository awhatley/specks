using System.Collections.Generic;
using System.Linq;

namespace Specks
{
    /// <summary>
    /// Provides a set of static extension methods supporting specifications.
    /// </summary>
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Filters a sequence of values based on a <see cref="Specification{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="query"/>.</typeparam>
        /// <param name="query">An <see cref="IQueryable{T}"/> to filter.</param>
        /// <param name="specification">A <see cref="Specification{T}"/> to test on each element.</param>
        /// <returns>An <see cref="IQueryable{T}"/> containing items that satisfy <paramref name="specification"/>.</returns>
        public static IQueryable<T> Matching<T>(this IQueryable<T> query, Specification<T> specification)
        {
            return specification.Filter(query);
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="Specification{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="query"/>.</typeparam>
        /// <param name="query">An <see cref="IEnumerable{T}"/> to filter.</param>
        /// <param name="specification">A <see cref="Specification{T}"/> to test on each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing items that satisfy <paramref name="specification"/>.</returns>
        public static IEnumerable<T> Matching<T>(this IEnumerable<T> query, Specification<T> specification)
        {
            return specification.Filter(query);
        }
    }
}