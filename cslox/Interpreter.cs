using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public class Interpreter : Expression.IVisitor<object?>
    {
        public object? visitBinary(Expression.Binary expression)
        {
            return null;
        }

        public object? visitTernary(Expression.Ternary expression)
        {
            return null;
        }

        public object? visitGrouping(Expression.Grouping expression)
        {
            return null;
        }

        public object? visitLiteral(Expression.Literal expression)
        {
            return expression.value;
        }

        public object? visitUnary(Expression.Unary expression)
        {
            return null;
        }
    }
}
