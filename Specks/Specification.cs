using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Specks.Logic;

namespace Specks
{
    /// <summary>
    /// Defines an atomic unit of logic used to specify criteria for selection and validation of candidate objects.
    /// </summary>
    /// <typeparam name="T">The candidate type used to satisfy the specification.</typeparam>
    public abstract class Specification<T>
    {
        #region Criteria Definition

        private Expression<Func<T, bool>> _criteria;

        /// <summary>
        /// Gets a LINQ expression representing the selection criteria encapsulated by
        /// this specification.
        /// </summary>
        public Expression<Func<T, bool>> Criteria
        {
            get { return _criteria ?? (_criteria = BuildCriteria()); }
        }

        /// <summary>
        /// When overridden in a derived class, builds the candidate selection criteria. Typically,
        /// this is done by returning a lambda expression.
        /// </summary>
        /// <returns>A LINQ expression encapsulating the selection criteria.</returns>
        protected abstract Expression<Func<T, bool>> BuildCriteria();

        #endregion

        #region Candidate Selection

        private Func<T, bool> _compiledExpression;

        private Func<T, bool> CompiledExpression
        {
            get { return _compiledExpression ?? (_compiledExpression = Criteria.Compile()); }
        }

        /// <summary>
        /// Evaluates the specified object against the candidate selection criteria.
        /// </summary>
        /// <param name="candidate">The candidate object to test.</param>
        /// <returns><b>true</b> if <paramref name="candidate"/> satisfies the criteria; otherwise, <b>false</b>.</returns>
        public bool IsSatisfiedBy(T candidate)
        {
            return CompiledExpression.Invoke(candidate);
        }

        /// <summary>
        /// Filters the specified list of candidates by eliminating objects not satisfying the
        /// current specification instance.
        /// </summary>
        /// <param name="candidates">The list of candidates to filter.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the candidates satisfying the specification.</returns>
        public IEnumerable<T> Filter(IEnumerable<T> candidates)
        {
            return candidates.Where(CompiledExpression);
        }

        /// <summary>
        /// Filters the specified list of candidates by eliminating objects not satisfying the
        /// current specification instance.
        /// </summary>
        /// <param name="candidates">The list of candidates to filter.</param>
        /// <returns>An <see cref="IQueryable{T}"/> of the candidates satisfying the specification.</returns>
        public IQueryable<T> Filter(IQueryable<T> candidates)
        {
            return candidates.Where(Criteria);
        }

        #endregion

        #region Composition Logic

        #region Negation

        /// <summary>
        /// Negates this instance, returning a new specification representing
        /// the logical negation of this instance's criteria.
        /// </summary>
        /// <returns>A new specification that represents the logical negation of this instance's criteria.</returns>
        public Specification<T> IsFalse()
        {
            return !this;
        }

        /// <summary>
        /// Negates the provided specification, returning a new specification
        /// representing the logical negation of its criteria.
        /// </summary>
        /// <param name="specification">The specification to negate.</param>
        /// <returns>A new specification that represents the logical negation of the original.</returns>
        public static Specification<T> operator !(Specification<T> specification)
        {
            return new Negation<T>(specification);
        }

        #endregion

        #region Conjunction

        /// <summary>
        /// Combines this specification with another, returning a new specification
        /// representing the logical conjunction of the two.
        /// </summary>
        /// <param name="specification">The specification to combine with this instance.</param>
        /// <returns>A new specification that represents the logical conjunction of the two.</returns>
        public Specification<T> And(Specification<T> specification)
        {
            return this & specification;
        }

        /// <summary>
        /// Combines this specification with another specification of a different type, using a
        /// conversion expression to translate the new candidate object type to the old type.
        /// </summary>
        /// <typeparam name="TValue">The new candidate type used to satisfy the specification.</typeparam>
        /// <param name="expression">An expression to translate the new candidate type to the old type.</param>
        /// <param name="specification">The specification to combine with this instance.</param>
        /// <returns>A new specification of the specified type.</returns>
        public Specification<T> And<TValue>(Expression<Func<T, TValue>> expression, Specification<TValue> specification)
        {
            return this & specification.From(expression);
        }

        /// <summary>
        /// Combines this specification with another negated specification, returning a new specification
        /// representing the logical conjunction of the two.
        /// </summary>
        /// <param name="specification">The specification to negate and combine with this instance.</param>
        /// <returns>A new specification that represents the logical conjunction of this instance
        /// and the negation of the other.</returns>
        public Specification<T> AndNot(Specification<T> specification)
        {
            return this & !specification;
        }

        /// <summary>
        /// Combines this specification with a negated specification of a different type, using a
        /// conversion expression to translate the new candidate object type to the old type.
        /// </summary>
        /// <typeparam name="TValue">The new candidate type used to satisfy the specification.</typeparam>
        /// <param name="expression">An expression to translate the new candidate type to the old type.</param>
        /// <param name="specification">The specification to combine with this instance.</param>
        /// <returns>A new specification of the specified type.</returns>
        public Specification<T> AndNot<TValue>(Expression<Func<T, TValue>> expression, Specification<TValue> specification)
        {
            return this & !specification.From(expression);
        }

        /// <summary>
        /// Combines this specification with another specification indicated by the type parameter, returning
        /// a new specification representing the logical conjunction of the two.
        /// </summary>
        /// <typeparam name="TSpecification">The type of specification to combine with this instance.</typeparam>
        /// <returns>A new specification that represents the logical conjunction of the two.</returns>
        public Specification<T> And<TSpecification>() where TSpecification : Specification<T>, new()
        {
            return this & new TSpecification();
        }

        /// <summary>
        /// Combines this specification with another negated specification indicated by the type parameter, 
        /// returning a new specification representing the logical conjunction of the two.
        /// </summary>
        /// <typeparam name="TSpecification">The type of specification to negate and combine with this instance.</typeparam>
        /// <returns>A new specification that represents the logical conjunction of this instance
        /// and the negation of the other.</returns>
        public Specification<T> AndNot<TSpecification>() where TSpecification : Specification<T>, new()
        {
            return this & !new TSpecification();
        }

        /// <summary>
        /// Combines two specifications, returning a new specification representing the logical conjunction
        /// of the two.
        /// </summary>
        /// <param name="left">The first specification to combine.</param>
        /// <param name="right">The second specification to combine.</param>
        /// <returns>A new specification that represents the logical conjunction of the two.</returns>
        public static Specification<T> operator &(Specification<T> left, Specification<T> right)
        {
            return new Conjunction<T>(left, right);
        }

        #endregion

        #region Disjunction

        /// <summary>
        /// Combines this specification with another, returning a new specification
        /// representing the logical disjunction of the two.
        /// </summary>
        /// <param name="specification">The specification to combine with this instance.</param>
        /// <returns>A new specification that represents the logical disjunction of the two.</returns>
        public Specification<T> Or(Specification<T> specification)
        {
            return this | specification;
        }

        /// <summary>
        /// Combines this specification with another specification of a different type, using a
        /// conversion expression to translate the new candidate object type to the old type.
        /// </summary>
        /// <typeparam name="TValue">The new candidate type used to satisfy the specification.</typeparam>
        /// <param name="expression">An expression to translate the new candidate type to the old type.</param>
        /// <param name="specification">The specification to combine with this instance.</param>
        /// <returns>A new specification of the specified type.</returns>
        public Specification<T> Or<TValue>(Expression<Func<T, TValue>> expression, Specification<TValue> specification)
        {
            return this | specification.From(expression);
        }

        /// <summary>
        /// Combines this specification with another negated specification, returning a new specification
        /// representing the logical disjunction of the two.
        /// </summary>
        /// <param name="specification">The specification to negate and combine with this instance.</param>
        /// <returns>A new specification that represents the logical disjunction of this instance
        /// and the negation of the other.</returns>
        public Specification<T> OrNot(Specification<T> specification)
        {
            return this | !specification;
        }

        /// <summary>
        /// Combines this specification with a negated specification of a different type, using a
        /// conversion expression to translate the new candidate object type to the old type.
        /// </summary>
        /// <typeparam name="TValue">The new candidate type used to satisfy the specification.</typeparam>
        /// <param name="expression">An expression to translate the new candidate type to the old type.</param>
        /// <param name="specification">The specification to combine with this instance.</param>
        /// <returns>A new specification of the specified type.</returns>
        public Specification<T> OrNot<TValue>(Expression<Func<T, TValue>> expression, Specification<TValue> specification)
        {
            return this | !(specification.From(expression));
        }

        /// <summary>
        /// Combines this specification with another specification indicated by the type parameter, returning
        /// a new specification representing the logical disjunction of the two.
        /// </summary>
        /// <typeparam name="TSpecification">The type of specification to combine with this instance.</typeparam>
        /// <returns>A new specification that represents the logical disjunction of the two.</returns>
        public Specification<T> Or<TSpecification>() where TSpecification : Specification<T>, new()
        {
            return this | new TSpecification();
        }

        /// <summary>
        /// Combines this specification with another negated specification indicated by the type parameter, 
        /// returning a new specification representing the logical disjunction of the two.
        /// </summary>
        /// <typeparam name="TSpecification">The type of specification to negate and combine with this instance.</typeparam>
        /// <returns>A new specification that represents the logical disjunction of this instance
        /// and the negation of the other.</returns>
        public Specification<T> OrNot<TSpecification>() where TSpecification : Specification<T>, new()
        {
            return this | !new TSpecification();
        }

        /// <summary>
        /// Combines two specifications, returning a new specification representing the logical disjunction
        /// of the two.
        /// </summary>
        /// <param name="left">The first specification to combine.</param>
        /// <param name="right">The second specification to combine.</param>
        /// <returns>A new specification that represents the logical disjunction of the two.</returns>
        public static Specification<T> operator |(Specification<T> left, Specification<T> right)
        {
            return new Disjunction<T>(left, right);
        }

        #endregion

        #region Exclusive Disjunction

        /// <summary>
        /// Combines this specification with another, returning a new specification
        /// representing the exclusive disjunction of the two.
        /// </summary>
        /// <param name="specification">The specification to combine with this instance.</param>
        /// <returns>A new specification that represents the exclusive disjunction of the two.</returns>
        public Specification<T> Xor(Specification<T> specification)
        {
            return this ^ specification;
        }

        /// <summary>
        /// Combines this specification with another negated specification, returning a new specification
        /// representing the exclusive disjunction of the two.
        /// </summary>
        /// <param name="specification">The specification to negate and combine with this instance.</param>
        /// <returns>A new specification that represents the exclusive disjunction of this instance
        /// and the negation of the other.</returns>
        public Specification<T> XorNot(Specification<T> specification)
        {
            return this ^ !specification;
        }

        /// <summary>
        /// Combines this specification with another specification indicated by the type parameter, returning
        /// a new specification representing the exclusive disjunction of the two.
        /// </summary>
        /// <typeparam name="TSpecification">The type of specification to combine with this instance.</typeparam>
        /// <returns>A new specification that represents the exclusive disjunction of the two.</returns>
        public Specification<T> Xor<TSpecification>() where TSpecification : Specification<T>, new()
        {
            return this ^ new TSpecification();
        }

        /// <summary>
        /// Combines this specification with another negated specification indicated by the type parameter, 
        /// returning a new specification representing the exclusive disjunction of the two.
        /// </summary>
        /// <typeparam name="TSpecification">The type of specification to negate and combine with this instance.</typeparam>
        /// <returns>A new specification that represents the exclusive disjunction of this instance
        /// and the negation of the other.</returns>
        public Specification<T> XorNot<TSpecification>() where TSpecification : Specification<T>, new()
        {
            return this ^ !new TSpecification();
        }

        /// <summary>
        /// Combines two specifications, returning a new specification representing the exclusive disjunction
        /// of the two.
        /// </summary>
        /// <param name="left">The first specification to combine.</param>
        /// <param name="right">The second specification to combine.</param>
        /// <returns>A new specification that represents the exclusive disjunction of the two.</returns>
        public static Specification<T> operator ^(Specification<T> left, Specification<T> right)
        {
            return new ExclusiveDisjunction<T>(left, right);
        }

        #endregion

        #region Quantifications

        /// <summary>
        /// Quantifies the current specification for any item in a collection of candidate objects,
        /// returning a new specification that takes in an <see cref="IEnumerable{T}"/> and is satisfied
        /// if any item in the collection satisfies this specification.
        /// </summary>
        /// <returns>A new specification that takes in an <see cref="IEnumerable{T}"/> and is satisfied
        /// if any item in the collection satisfies this specification.</returns>
        public Specification<IEnumerable<T>> ForAny()
        {
            return new ExistentialQuantification<T>(this);
        }

        /// <summary>
        /// Quantifies the current specification for all items in a collection of candidate objects,
        /// returning a new specification that takes in an <see cref="IEnumerable{T}"/> and is satisfied
        /// if all items in the collection satisfy this specification.
        /// </summary>
        /// <returns>A new specification that takes in an <see cref="IEnumerable{T}"/> and is satisfied
        /// if all items in the collection satisfy this specification.</returns>
        public Specification<IEnumerable<T>> ForAll()
        {
            return new UniversalQuantification<T>(this);
        }

        /// <summary>
        /// Quantifies the current specification for exactly one item in a collection of candidate objects,
        /// returning a new specification that takes in an <see cref="IEnumerable{T}"/> and is satisfied
        /// if exactly one item in the collection satisfies this specification.
        /// </summary>
        /// <returns>A new specification that takes in an <see cref="IEnumerable{T}"/> and is satisfied
        /// if exactly one item in the collection satisfies this specification.</returns>
        public Specification<IEnumerable<T>> ForOne()
        {
            return new UniqueQuantification<T>(this);
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts a specification of one type to a specification of another type using a
        /// conversion expression to translate the new candidate object type to the old type.
        /// </summary>
        /// <typeparam name="TNew">The new candidate type used to satisfy the specification.</typeparam>
        /// <param name="converter">An expression to translate the new candidate type to the old type.</param>
        /// <returns>A new specification of the specified type.</returns>
        public Specification<TNew> From<TNew>(Expression<Func<TNew, T>> converter)
        {
            return new Conversion<TNew, T>(this, converter);
        }

        #endregion

        #endregion

        #region Type Conversion

        /// <summary>
        /// Converts a specification instance into an <see cref="Expression{TDelegate}"/> expression tree.
        /// </summary>
        /// <param name="specification">The specification instance.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> expression tree.</returns>
        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        {
            return specification.Criteria;
        }

        /// <summary>
        /// Converts a strongly-typed lambda expression tree into a specification instance.
        /// </summary>
        /// <param name="expression">The expression to convert.</param>
        /// <returns>A new specification representing the criteria of the expression.</returns>
        public static implicit operator Specification<T>(Expression<Func<T, bool>> expression)
        {
            return new Proposition<T>(expression);
        }

        #endregion

        //#region Subsumption

        //public virtual bool IsSpecialCaseOf(Specification<T> specification)
        //{
        //    return false;
        //}

        //public virtual bool IsGeneralizationOf(Specification<T> specification)
        //{
        //    return specification.IsSpecialCaseOf(this);
        //}

        //#endregion

        //#region Partial Fulfillment

        //public virtual Specification<T> RemainderUnsatisfiedBy(T candidate)
        //{
        //    return IsSatisfiedBy(candidate) ? null : this;
        //}

        //#endregion
    }
}