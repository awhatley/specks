using System;
using System.Linq.Expressions;

using Specks.Expressions;

namespace Specks
{
    /// <summary>
    /// Provides the abstract base class for a specification that is made up of 
    /// multiple component specifications.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
    public abstract class CompositeSpecification<T> : Specification<T>
    {
        /// <summary>
        /// When overridden in a derived class, constructs the composite specification that comprises
        /// this specification. Typically, this is done using the <see cref="Specify{T}"/> class.
        /// </summary>
        /// <returns>The composite specification represented by this instance.</returns>
        protected abstract Specification<T> BuildComposite();

        /// <summary>
        /// Builds the candidate selection criteria for the inner composite specification.
        /// </summary>
        /// <returns>A LINQ expression encapsulating the selection criteria.</returns>
        protected override sealed Expression<Func<T, bool>> BuildCriteria()
        {
            return BuildComposite().Criteria;
        }
    }

    /// <summary>
    /// Provides the abstract base class for a specification that uses an inner specification of one
    /// type to match an object of a different type.
    /// </summary>
    /// <typeparam name="TOuter">The candidate type used to satisfy the outer specification.</typeparam>
    /// <typeparam name="TInner">The candidate type used to satisfy the inner specification.</typeparam>
    public abstract class CompositeSpecification<TOuter, TInner> : Specification<TOuter>
    {
        /// <summary>
        /// When overridden in a derived class, constructs the composite specification that comprises
        /// this specification. Typically, this is done using the <see cref="Specify{T}"/> class.
        /// </summary>
        /// <returns>The composite specification represented by this instance.</returns>
        protected abstract Specification<TInner> BuildInnerSpecification();

        /// <summary>
        /// When overridden in a derived class, constructs a conversion expression used to translate
        /// the outer candidate object type into the type required by the inner specification. Typically,
        /// this is done by returning a lambda expression.
        /// </summary>
        /// <returns>An expression to translate the new candidate type to the old type.</returns>
        protected abstract Expression<Func<TOuter, TInner>> BuildConversionExpression();

        /// <summary>
        /// Builds the candidate selection criteria for the inner composite specification.
        /// </summary>
        /// <returns>A LINQ expression encapsulating the selection criteria.</returns>
        protected override sealed Expression<Func<TOuter, bool>> BuildCriteria()
        {
            var transform = BuildConversionExpression();
            var criteria = BuildInnerSpecification().Criteria;
            var parameter = transform.Parameters[0];
            var body = ExpressionReplacer.Replace(criteria.Body, criteria.Parameters[0], transform.Body);

            return Expression.Lambda<Func<TOuter, bool>>(body, parameter);
        }
    }
}