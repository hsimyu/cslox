/* This is auto-generated code. Do not edit. */
namespace cslox
{

    public abstract class Expression
    {
        public interface IVisitor<R>
        {
            R visitBinary(Binary expression);
            R visitGrouping(Grouping expression);
            R visitLiteral(Literal expression);
            R visitUnary(Unary expression);
        }
        public abstract R accept<R>(IVisitor<R> visitor);

        public class Binary : Expression
        {
            internal Binary(Expression left, Token op, Expression right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitBinary(this);
            }
            public Expression left;
            public Token op;
            public Expression right;
        }

        public class Grouping : Expression
        {
            internal Grouping(Expression exp)
            {
                this.exp = exp;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitGrouping(this);
            }
            public Expression exp;
        }

        public class Literal : Expression
        {
            internal Literal(object? value)
            {
                this.value = value;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitLiteral(this);
            }
            public object? value;
        }

        public class Unary : Expression
        {
            internal Unary(Token op, Expression right)
            {
                this.op = op;
                this.right = right;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitUnary(this);
            }
            public Token op;
            public Expression right;
        }

    }
} // namespace cslox
