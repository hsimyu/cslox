﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public class AstPrinter : Expression.IVisitor<string>
    {
        public string print(Expression expr)
        {
            return expr.accept(this);
        }

        public string visitAssign(Expression.Assign expr)
        {
            return parenthesize($"= {expr.name}", expr.value);
        }
        public string visitBinary(Expression.Binary expr)
        {
            return parenthesize(expr.op.lexeme, expr.left, expr.right);
        }
        public string visitTernary(Expression.Ternary expr)
        {
            return parenthesize(expr.op.lexeme, expr.cond, expr.first, expr.second);
        }
        public string visitCall(Expression.Call expr)
        {
            return parenthesize(expr.callee.accept(this), expr.arguments.ToArray());
        }
        public string visitGet(Expression.Get expr)
        {
            return $"{expr.obj.accept(this)}.{expr.name.lexeme}";
        }
        public string visitSet(Expression.Set expr)
        {
            return $"(= {expr.obj.accept(this)}.{expr.name.lexeme} {expr.value.accept(this)})";
        }
        public string visitThis(Expression.This expr)
        {
            return "this";
        }
        public string visitSuper(Expression.Super expr)
        {
            return "super";
        }
        public string visitGrouping(Expression.Grouping expr)
        {
            return parenthesize("group", expr.exp);
        }
        public string visitLogical(Expression.Logical expr)
        {
            return parenthesize(expr.op.lexeme, expr.left, expr.right);
        }
        public string visitLiteral(Expression.Literal expr)
        {
            if (expr.value == null) return "nil";
            return expr.value.ToString() ?? "";
        }
        public string visitUnary(Expression.Unary expr)
        {
            return parenthesize(expr.op.lexeme, expr.right);
        }
        public string visitVariable(Expression.Variable variable)
        {
            return variable.name.lexeme;
        }

        private string parenthesize(string name, params Expression[] expressions)
        {
            var builder = new StringBuilder();
            builder.Append("(").Append(name);

            foreach (var e in expressions)
            {
                builder.Append(' ');
                builder.Append(e.accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
