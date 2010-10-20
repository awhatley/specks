using System;
using System.Linq.Expressions;

namespace Specks.Logic
{
    /// <summary>
    /// A specification that is satisfied using a conversion method to satisfy an inner specification 
    /// of a different type.
    /// </summary>
    /// <typeparam name="TOuter">The candidate type used to satisfy the outer specification.</typeparam>
    /// <typeparam name="TInner">The candidate type used to satisfy the inner specification.</typeparam>
    internal class Conversion<TOuter, TInner> : CompositeSpecification<TOuter, TInner>
    {
        private Specification<TInner> Inner { get; set; }
        private Expression<Func<TOuter, TInner>> Converter { get; set; }

        public Conversion(Specification<TInner> inner, Expression<Func<TOuter, TInner>> converter)
        {
            Inner = inner;
            Converter = converter;
        }

        protected override Specification<TInner> BuildInnerSpecification()
        {
            return Inner;
        }

        protected override Expression<Func<TOuter, TInner>> BuildConversionExpression()
        {
            return Converter;
        }
    }
}