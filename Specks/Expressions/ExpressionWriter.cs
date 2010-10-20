using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Specks.Expressions
{
    /// <summary>
    /// Writes out an expression tree in a C# style syntax
    /// </summary>
    public class ExpressionWriter : ExpressionVisitor
    {
        private const int IndentSize = 2;
        private static readonly char[] Splitters = new[] { '\n', '\r' };
        private static readonly char[] Special = new[] { '\n', '\n', '\\' };

        private readonly TextWriter _writer;
        private int _depth;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionWriter"/> class
        /// using the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> implementation to write to.</param>
        protected ExpressionWriter(TextWriter writer)
        {
            _writer = writer;
        }

        /// <summary>
        /// Writes the specified expression to <see cref="Console.Out"/>.
        /// </summary>
        /// <param name="expression">The expression to write.</param>
        public static void Write(Expression expression)
        {
            Write(Console.Out, expression);
        }

        /// <summary>
        /// Writes the specified expression to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> implementation to write to.</param>
        /// <param name="expression">The expression to write.</param>
        public static void Write(TextWriter writer, Expression expression)
        {
            new ExpressionWriter(writer).Visit(expression);
        }

        /// <summary>
        /// Writes the specified expression to a string and returns the value.
        /// </summary>
        /// <param name="expression">The expression to write.</param>
        /// <returns>A string representation of the expression.</returns>
        public static string WriteToString(Expression expression)
        {
            var sw = new StringWriter();
            Write(sw, expression);
            return sw.ToString();
        }

        private enum Indentation
        {
            Same,
            Inner,
            Outer
        }

        private void WriteLine(Indentation style)
        {
            _writer.WriteLine();
            Indent(style);


            var spaces = _depth * IndentSize;
            for(var i = 0; i < spaces; i++)
                _writer.Write(" ");
        }

        private void Write(string text)
        {
            if(text.IndexOf('\n') >= 0)
            {
                var lines = text.Split(Splitters, StringSplitOptions.RemoveEmptyEntries);
                var lineCount = lines.Length;
                for(var i = 0; i < lineCount; i++)
                {
                    Write(lines[i]);
                    if(i < lineCount - 1)
                        WriteLine(Indentation.Same);
                }
            }

            else
            {
                _writer.Write(text);
            }
        }

        private void Indent(Indentation style)
        {
            if(style == Indentation.Inner)
            {
                _depth++;
            }
            else if(style == Indentation.Outer)
            {
                _depth--;
                Debug.Assert(_depth >= 0);
            }
        }

        /// <summary>
        /// Writes out the children of the <see cref="BinaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="b">The expression to visit.</param>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch(b.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    Visit(b.Left);
                    Write("[");
                    Visit(b.Right);
                    Write("]");
                    break;

                case ExpressionType.Power:
                    Write("POW(");
                    Visit(b.Left);
                    Write(", ");
                    Visit(b.Right);
                    Write(")");
                    break;

                default:
                    Visit(b.Left);
                    Write(" ");
                    Write(GetOperator(b.NodeType));
                    Write(" ");
                    Visit(b.Right);
                    break;
            }

            return b;
        }

        /// <summary>
        /// Writes out the children of the <see cref="UnaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="u">The expression to visit.</param>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch(u.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    Write("((");
                    Write(GetTypeName(u.Type));
                    Write(")");
                    Visit(u.Operand);
                    Write(")");
                    break;
                case ExpressionType.ArrayLength:
                    Visit(u.Operand);
                    Write(".Length");
                    break;
                case ExpressionType.Quote:
                    Visit(u.Operand);
                    break;
                case ExpressionType.TypeAs:
                    Visit(u.Operand);
                    Write(" as ");
                    Write(GetTypeName(u.Type));
                    break;
                case ExpressionType.UnaryPlus:
                    Visit(u.Operand);
                    break;
                default:
                    Write(GetOperator(u.NodeType));
                    Visit(u.Operand);
                    break;
            }
            return u;
        }

        /// <summary>
        /// Writes out the children of the <see cref="ConditionalExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="c">The expression to visit.</param>
        protected override Expression VisitConditional(ConditionalExpression c)
        {
            Visit(c.Test);
            WriteLine(Indentation.Inner);
            Write("? ");
            Visit(c.IfTrue);
            WriteLine(Indentation.Same);
            Write(": ");
            Visit(c.IfFalse);
            Indent(Indentation.Outer);
            return c;
        }

        /// <summary>
        /// Writes out the children of the <see cref="ConstantExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="c">The expression to visit.</param>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if(c.Value == null)
            {
                Write("null");
            }
            else if(c.Type == typeof(string))
            {
                var value = c.Value.ToString();
                if(value.IndexOfAny(Special) >= 0)
                    Write("@");
                Write("\"");
                Write(c.Value.ToString());
                Write("\"");
            }
            else if(c.Type == typeof(DateTime))
            {
                Write("new DateTime(\"");
                Write(c.Value.ToString());
                Write("\")");
            }
            else if(c.Type.IsArray)
            {
                var elementType = c.Type.GetElementType();
                VisitNewArray(
                    Expression.NewArrayInit(
                        elementType,
                        ((IEnumerable)c.Value).OfType<object>().Select(v => (Expression)Expression.Constant(v, elementType))
                        ));
            }
            else
            {
                Write(c.Value.ToString());
            }
            return c;
        }

        /// <summary>
        /// Writes out the children of the <see cref="ElementInit"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="initializer">The expression to visit.</param>
        protected override ElementInit VisitElementInit(ElementInit initializer)
        {
            if(initializer.Arguments.Count > 1)
            {                
                Write("{");

                var argCount = initializer.Arguments.Count;
                for(var i = 0; i < argCount; i++)
                {
                    Visit(initializer.Arguments[i]);
                    if(i < argCount - 1)
                        Write(", ");
                }

                Write("}");
            }
            else
            {
                Visit(initializer.Arguments[0]);
            }

            return initializer;
        }

        /// <summary>
        /// Writes out the children of the <see cref="InvocationExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="iv">The expression to visit.</param>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            Write("Invoke(");
            WriteLine(Indentation.Inner);
            Visit(iv.Arguments);
            Write(", ");
            WriteLine(Indentation.Same);
            Visit(iv.Expression);
            WriteLine(Indentation.Same);
            Write(")");
            Indent(Indentation.Outer);
            return iv;
        }

        /// <summary>
        /// Writes out the children of the <see cref="Expression{TDelegate}"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="lambda">The expression to visit.</param>
        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            if(lambda.Parameters.Count != 1)
            {
                Write("(");
                var parameterCount = lambda.Parameters.Count;
                for(var i = 0; i < parameterCount; i++)
                {
                    Write(lambda.Parameters[i].Name);
                    if(i < parameterCount - 1)
                        Write(", ");
                }

                Write(")");
            }
            else
            {
                Write(lambda.Parameters[0].Name);
            }

            Write(" => ");
            Visit(lambda.Body);
            return lambda;
        }

        /// <summary>
        /// Writes out the children of the <see cref="ListInitExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="init">The expression to visit.</param>
        protected override Expression VisitListInit(ListInitExpression init)
        {
            Visit(init.NewExpression);
            Write(" {");
            WriteLine(Indentation.Inner);
            Visit(init.Initializers, VisitElementInit);
            WriteLine(Indentation.Outer);
            Write("}");
            return init;
        }

        /// <summary>
        /// Writes out the children of the <see cref="MemberExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="m">The expression to visit.</param>
        protected override Expression VisitMember(MemberExpression m)
        {
            Visit(m.Expression);
            Write(".");
            Write(m.Member.Name);
            return m;
        }

        /// <summary>
        /// Writes out the children of the <see cref="MemberAssignment"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="assignment">The expression to visit.</param>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Write(assignment.Member.Name);
            Write(" = ");
            Visit(assignment.Expression);
            return assignment;
        }

        /// <summary>
        /// Writes out the children of the <see cref="MemberInitExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="init">The expression to visit.</param>
        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            Visit(init.NewExpression);
            Write(" {");
            WriteLine(Indentation.Inner);
            Visit(init.Bindings, VisitMemberBinding);
            WriteLine(Indentation.Outer);
            Write("}");
            return init;
        }

        /// <summary>
        /// Writes out the children of the <see cref="MemberListBinding"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="binding">The expression to visit.</param>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            Write(binding.Member.Name);
            Write(" = {");
            WriteLine(Indentation.Inner);
            Visit(binding.Initializers, VisitElementInit);
            WriteLine(Indentation.Outer);
            Write("}");
            return binding;
        }

        /// <summary>
        /// Writes out the children of the <see cref="MemberMemberBinding"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="binding">The expression to visit.</param>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            Write(binding.Member.Name);
            Write(" = {");
            WriteLine(Indentation.Inner);
            Visit(binding.Bindings, VisitMemberBinding);
            WriteLine(Indentation.Outer);
            Write("}");
            return binding;
        }

        /// <summary>
        /// Writes out the children of the <see cref="MethodCallExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="m">The expression to visit.</param>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if(m.Object != null)
            {
                Visit(m.Object);
            }
            else
            {
                Write(GetTypeName(m.Method.DeclaringType));
            }
            Write(".");
            Write(m.Method.Name);
            Write("(");
            if(m.Arguments.Count > 1)
                WriteLine(Indentation.Inner);
            Visit(m.Arguments);
            if(m.Arguments.Count > 1)
                WriteLine(Indentation.Outer);
            Write(")");
            return m;
        }

        /// <summary>
        /// Writes out the children of the <see cref="NewExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="nex">The expression to visit.</param>
        protected override Expression VisitNew(NewExpression nex)
        {
            Write("new ");
            Write(GetTypeName(nex.Constructor.DeclaringType));
            Write("(");
            if(nex.Arguments.Count > 1)
                WriteLine(Indentation.Inner);
            Visit(nex.Arguments);
            if(nex.Arguments.Count > 1)
                WriteLine(Indentation.Outer);
            Write(")");
            return nex;
        }

        /// <summary>
        /// Writes out the children of the <see cref="NewArrayExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="na">The expression to visit.</param>
        protected override Expression VisitNewArray(NewArrayExpression na)
        {
            Write("new ");
            Write(GetTypeName(na.Type.GetElementType()));
            Write("[] {");
            if(na.Expressions.Count > 1)
                WriteLine(Indentation.Inner);
            Visit(na.Expressions);
            if(na.Expressions.Count > 1)
                WriteLine(Indentation.Outer);
            Write("}");
            return na;
        }

        /// <summary>
        /// Writes out the children of the <see cref="ParameterExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="p">The expression to visit.</param>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            Write(p.Name);
            return p;
        }

        /// <summary>
        /// Writes out the children of the <see cref="TypeBinaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.
        /// </returns>
        /// <param name="b">The expression to visit.</param>
        protected override Expression VisitTypeBinary(TypeBinaryExpression b)
        {
            Visit(b.Expression);
            Write(" is ");
            Write(GetTypeName(b.TypeOperand));
            return b;
        }

        private static string GetOperator(ExpressionType type)
        {
            switch(type)
            {
                case ExpressionType.Not:
                    return "!";

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";

                case ExpressionType.Divide:
                    return "/";

                case ExpressionType.Modulo:
                    return "%";

                case ExpressionType.And:
                    return "&";

                case ExpressionType.AndAlso:
                    return "&&";

                case ExpressionType.Or:
                    return "|";

                case ExpressionType.OrElse:
                    return "||";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.Equal:
                    return "==";

                case ExpressionType.NotEqual:
                    return "!=";

                case ExpressionType.Coalesce:
                    return "??";

                case ExpressionType.RightShift:
                    return ">>";

                case ExpressionType.LeftShift:
                    return "<<";

                case ExpressionType.ExclusiveOr:
                    return "^";

                default:
                    return null;
            }
        }

        private static string GetTypeName(Type type)
        {
            var name = type.Name.Replace('+', '.');

            var isGeneric = name.IndexOf('`');
            if(isGeneric > 0)
                name = name.Substring(0, isGeneric);

            if(type.IsGenericType || type.IsGenericTypeDefinition)
            {
                var sb = new StringBuilder();
                sb.Append(name);
                sb.Append("<");

                var args = type.GetGenericArguments();
                var argCount = args.Length;
                for(var i = 0; i < argCount; i++)
                {
                    if(i > 0)
                        sb.Append(",");

                    if(type.IsGenericType)
                        sb.Append(GetTypeName(args[i]));
                }

                sb.Append(">");
                name = sb.ToString();
            }

            return name;
        }
    }
}