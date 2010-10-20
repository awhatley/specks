using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

namespace Specks.Tests
{
    [TestFixture]
    public class ParameterRebinderPerformanceTest
    {
        const int ITERATIONS = 100;

        readonly Expression<Func<int, bool>> greaterThanZero = i => i > 0 && i != 0;
        readonly Expression<Func<int, bool>> lessThanZero = i => i < 0 && i != 0;

        [Test]
        public void Manual()
        {
            Time("Manual", () => Or(greaterThanZero, lessThanZero));

            var expr = Or(greaterThanZero, lessThanZero);

            Console.WriteLine(expr);

            Time("ManualCompile", () => expr.Compile());
        }

        [Test]
        public void ParameterRebinder()
        {
            Time("ParameterRebinder", () => greaterThanZero.OrPR(lessThanZero));

            var expr = greaterThanZero.OrPR(lessThanZero);

            Console.WriteLine(expr);

            Time("ParameterRebinderCompile", () => expr.Compile());
        }

        [Test]
        public void ParameterRebinderSingle()
        {
            Time("ParameterRebinderSingle", () => greaterThanZero.OrPRS(lessThanZero));

            var expr = greaterThanZero.OrPRS(lessThanZero);

            Console.WriteLine(expr);

            Time("ParameterRebinderSingleCompile", () => expr.Compile());
        }

        [Test]
        public void ParameterRebinderSelective()
        {
            Time("ParameterRebinderSelective", () => greaterThanZero.OrPRSS(lessThanZero));

            var expr = greaterThanZero.OrPRSS(lessThanZero);

            Console.WriteLine(expr);

            Time("ParameterRebinderSelectiveCompile", () => expr.Compile());
        }

        [Test]
        public void InvocationExpander()
        {
            Time("InvocationExpander", () => Or(greaterThanZero, lessThanZero).ExpandInvocations());

            var expr = Or(greaterThanZero, lessThanZero).ExpandInvocations();

            Console.WriteLine(expr);

            Time("InvocationExpanderCompile", () => expr.Compile());
        }

        [Test]
        public void FindConstantInExpression()
        {
            const int x = 4;
            Expression<Func<int, bool>> expr = i => !x.Equals(i);

            var body = (UnaryExpression)expr.Body;
            var call = (MethodCallExpression)body.Operand;

            Console.WriteLine(call.Object);
        }

        private static void Time(string name, Action action)
        {
            var watch = new Stopwatch();
            watch.Start();
            for(var i = 0; i < ITERATIONS; i++)
            {
                action();
            }
            watch.Stop();

            Console.WriteLine(name + " " + watch.Elapsed);
        }

        private static Expression<T> Or<T>(Expression<T> left, Expression<T> right)
        {
            var parameter = Expression.Parameter(left.Parameters[0].Type, "i");
            
            return Expression.Lambda<T>(
                Expression.OrElse(
                    Expression.Invoke(left, parameter),
                    Expression.Invoke(right, parameter)), parameter);
        }
    }

    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Expression exp, Dictionary<ParameterExpression, ParameterExpression> map)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if(_map.TryGetValue(p, out replacement))
                p = replacement;

            return base.VisitParameter(p);
        }
    }

    internal class ParameterRebinderSingle : ExpressionVisitor
    {
        private readonly ParameterExpression _first;

        private ParameterRebinderSingle(ParameterExpression first)
        {
            _first = first;
        }

        public static Expression ReplaceParameters(Expression exp, ParameterExpression first)
        {
            return new ParameterRebinderSingle(first).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return _first;
        }
    }

    internal class ParameterRebinderSelective : ExpressionVisitor
    {
        private readonly ParameterExpression _first;
        private readonly ParameterExpression _second;

        private ParameterRebinderSelective(ParameterExpression first, ParameterExpression second)
        {
            _first = first;
            _second = second;
        }

        public static Expression ReplaceParameters(Expression exp, ParameterExpression first, ParameterExpression second)
        {
            return new ParameterRebinderSelective(first, second).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return (p == _second) ? _first : p;
        }
    }

    internal sealed class InvocationExpander : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        private readonly Expression _expansion;
        private readonly InvocationExpander _previous;
        private static readonly InvocationExpander s_singleton = new InvocationExpander();

        private InvocationExpander(ParameterExpression parameter, Expression expansion, InvocationExpander previous)
        {
            Extensions.CheckArgumentNotNull(parameter, "parameter");
            Extensions.CheckArgumentNotNull(expansion, "expansion");
            Extensions.CheckArgumentNotNull(previous, "previous");

            _parameter = parameter;
            _expansion = expansion;
            _previous = previous;
        }

        private InvocationExpander()
        {
        }

        internal static Expression Expand(Expression expression)
        {
            return s_singleton.Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            var expander = this;
            while(null != expander)
            {
                if(expander._parameter == p)
                {
                    return Visit(expander._expansion);
                }
                expander = expander._previous;
            }
            return base.VisitParameter(p);
        }

        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            if(iv.Expression.NodeType == ExpressionType.Lambda)
            {
                var lambda = (LambdaExpression)iv.Expression;
                return lambda
                    .Parameters
                    // zip together parameters and the corresponding argument values
                    .Zip(iv.Arguments, (p, e) => new { Parameter = p, Expansion = e })
                    // add to the stack of available parameters bindings (this class doubles as an immutable stack)
                    .Aggregate(this, (previous, pair) => new InvocationExpander(pair.Parameter, pair.Expansion, previous))
                    // visit the body of the lambda using an expander including the new parameter bindings
                    .Visit(lambda.Body);
            }
            return base.VisitInvocation(iv);
        }
    }

    public static class Extensions
    {
        public static Expression<T> ExpandInvocations<T>(this Expression<T> expression)
        {
            return (Expression<T>)InvocationExpander.Expand(expression);
        }

        internal static void CheckArgumentNotNull<T>(T argumentValue, string argumentName)
            where T : class
        {
            if(null == argumentValue)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        private static Expression<T> ComposePR<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            var secondBody = ParameterRebinder.ReplaceParameters(second.Body, map);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<T> OrPR<T>(this Expression<T> first, Expression<T> second)
        {
            return first.ComposePR(second, Expression.OrElse);
        }

        private static Expression<T> ComposePRS<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var secondBody = ParameterRebinderSingle.ReplaceParameters(second.Body, first.Parameters[0]);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<T> OrPRS<T>(this Expression<T> first, Expression<T> second)
        {
            return first.ComposePRS(second, Expression.OrElse);
        }

        private static Expression<T> ComposePRSS<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var secondBody = ParameterRebinderSelective.ReplaceParameters(second.Body, first.Parameters[0], second.Parameters[0]);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<T> OrPRSS<T>(this Expression<T> first, Expression<T> second)
        {
            return first.ComposePRSS(second, Expression.OrElse);
        }
    }
}