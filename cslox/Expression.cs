/* This is auto-generated code. Do not edit. */
namespace cslox
{

    public abstract class Expression
    {
        public interface IVisitor<R>
        {
            R visitAssign(Assign expression);
            R visitBinary(Binary expression);
            R visitTernary(Ternary expression);
            R visitGrouping(Grouping expression);
            R visitLiteral(Literal expression);
            R visitUnary(Unary expression);
            R visitVariable(Variable expression);
        }
        public abstract R accept<R>(IVisitor<R> visitor);

        public class Assign : Expression
        {
            internal Assign(Token name, Expression value)
            {
                this.name = name;
                this.value = value;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitAssign(this);
            }
            public Token name;
            public Expression value;
        }

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

        public class Ternary : Expression
        {
            internal Ternary(Expression cond, Token op, Expression first, Expression second)
            {
                this.cond = cond;
                this.op = op;
                this.first = first;
                this.second = second;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitTernary(this);
            }
            public Expression cond;
            public Token op;
            public Expression first;
            public Expression second;
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

        public class Variable : Expression
        {
            internal Variable(Token name)
            {
                this.name = name;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitVariable(this);
            }
            public Token name;
        }

    }
} // namespace cslox
