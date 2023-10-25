using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public class Interpreter : Expression.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        public void interpret(List<Stmt> stmts)
        {
            try
            {
                foreach(Stmt stmt in stmts)
                {
                    execute(stmt);
                }
            }
            catch (RuntimeError re)
            {
                Program.runtimeError(re);
            }
        }

        void execute(Stmt stmt)
        {
            stmt.accept(this);
        }

        public object? visitExpressionStmt(Stmt.ExpressionStmt stmt)
        {
            // NOTE: object? を返しているのは型引数に void を使えないためで、意味はない 
            evaluate(stmt.expression);
            return null;
        }

        public object? visitPrintStmt(Stmt.PrintStmt stmt)
        {
            var value = evaluate(stmt.expression);
            Console.WriteLine(stringify(value));
            return null;
        }

        public object? visitVarStmt(Stmt.VarStmt stmt)
        {
            return null;
        }

        public string stringify(object? value)
        {
            if (value == null) return "nil";

            if (value is double)
            {
                var text = value.ToString() ?? "";
                // if (text.EndsWith(".0"))
                // {
                //     text = text.Substring(0, text.Length - 2);
                // }
                return text;
            }

            return value.ToString() ?? "";
        }

        public object? visitBinary(Expression.Binary expression)
        {
            var left = evaluate(expression.left);
            var right = evaluate(expression.right);

            switch (expression.op.type)
            {
                case TokenType.BANG_EQUAL:
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);
                case TokenType.GREATER:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }

                    throw new RuntimeError(expression.op, "Operands must be two numbers or two strings.");
            }

            return null;
        }

        public object? visitTernary(Expression.Ternary expression)
        {
            var cond = evaluate(expression.cond);
            if (isTruthy(cond))
            {
                return evaluate(expression.first);
            }
            else
            {
                return evaluate(expression.second);
            }
        }

        public object? visitGrouping(Expression.Grouping expression)
        {
            return evaluate(expression.exp);
        }

        public object? visitLiteral(Expression.Literal expression)
        {
            return expression.value;
        }

        public object? visitUnary(Expression.Unary expression)
        {
            object? right = evaluate(expression.right);

            switch (expression.op.type)
            {
                case TokenType.BANG:
                    return !isTruthy(right);
                case TokenType.MINUS:
                    checkNumberOperand(expression.op, expression.right);
                    return -(double)(right ?? 0.0);
            }

            return null;
        }

        public object? visitVariable(Expression.Variable variable)
        {
            return null;
        }

        object? evaluate(Expression expr)
        {
            return expr.accept(this);
        }

        bool isTruthy(object? value)
        {
            if (value == null) return false;
            if (value is bool) return (bool)value;

            // bool 型でなければ全て true
            return true;
        }

        bool isEqual(object? lhs, object? rhs)
        {
            if (lhs == null && rhs == null) return true;
            if (lhs == null) return false;
            return lhs.Equals(rhs);
        }

        void checkNumberOperand(Token op, object? operand)
        {
            if (operand is double)
            {
                return;
            }
            throw new RuntimeError(op, "Operand must be a number");
        }

        void checkNumberOperands(Token op, object? lhs, object? rhs)
        {
            if ((lhs is double) && (rhs is double))
            {
                return;
            }
            throw new RuntimeError(op, "Operand must be numbers");
        }
    }
}
