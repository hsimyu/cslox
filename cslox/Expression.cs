/* This is auto-generated code. Do not edit. */
namespace cslox
{

    abstract class Expression
    {
    }

    internal class Binary : Expression
    {
        internal Binary(Expression left, Token op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
        Expression left;
        Token op;
        Expression right;
    }

    internal class Grouping : Expression
    {
        internal Grouping(Expression exp)
        {
            this.exp = exp;
        }
        Expression exp;
    }

    internal class Literal : Expression
    {
        internal Literal(object value)
        {
            this.value = value;
        }
        object value;
    }

    internal class Unary : Expression
    {
        internal Unary(Token op, Expression right)
        {
            this.op = op;
            this.right = right;
        }
        Token op;
        Expression right;
    }

} // namespace cslox
