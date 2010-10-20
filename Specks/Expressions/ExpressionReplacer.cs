using System.Linq.Expressions;

namespace Specks.Expressions
{
    /// <summary>
    /// Replaces references to one specific instance of an expression node with another node
    /// </summary>
    public class ExpressionReplacer : ExpressionVisitor
    {
        private readonly Expression _searchFor;
        private readonly Expression _replaceWith;

        private ExpressionReplacer(Expression searchFor, Expression replaceWith)
        {
            _searchFor = searchFor;
            _replaceWith = replaceWith;
        }

        /// <summary>
        /// Searches the provided expression for any instance of an expression node and replaces
        /// it with another.
        /// </summary>
        /// <param name="expression">The root expression from which to begin the search.</param>
        /// <param name="searchFor">The expression to search for.</param>
        /// <param name="replaceWith">The expression to use as a replacement.</param>
        /// <returns></returns>
        public static Expression Replace(Expression expression, Expression searchFor, Expression replaceWith)
        {
            return new ExpressionReplacer(searchFor, replaceWith).Visit(expression);
        }

        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns 
        /// the original expression.
        /// </returns>
        /// <param name="expression">The expression to visit.</param>
        public override Expression Visit(Expression expression)
        {
            return expression == _searchFor ? _replaceWith : 
                base.Visit(expression);
        }
    }
}