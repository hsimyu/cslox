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
            R visitCall(Call expression);
            R visitGrouping(Grouping expression);
            R visitLogical(Logical expression);
            R visitLiteral(Literal expression);
            R visitUnary(Unary expression);
            R visitVariable(Variable expression);
            R visitGet(Get expression);
            R visitSet(Set expression);
            R visitThis(This expression);
            R visitSuper(Super expression);
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

        public class Call : Expression
        {
            internal Call(Expression callee, Token paren, List<Expression> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitCall(this);
            }
            public Expression callee;
            public Token paren;
            public List<Expression> arguments;
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

        public class Logical : Expression
        {
            internal Logical(Expression left, Token op, Expression right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitLogical(this);
            }
            public Expression left;
            public Token op;
            public Expression right;
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

        public class Get : Expression
        {
            internal Get(Expression obj, Token name)
            {
                this.obj = obj;
                this.name = name;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitGet(this);
            }
            public Expression obj;
            public Token name;
        }

        public class Set : Expression
        {
            internal Set(Expression obj, Token name, Expression value)
            {
                this.obj = obj;
                this.name = name;
                this.value = value;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitSet(this);
            }
            public Expression obj;
            public Token name;
            public Expression value;
        }

        public class This : Expression
        {
            internal This(Token keyword)
            {
                this.keyword = keyword;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitThis(this);
            }
            public Token keyword;
        }

        public class Super : Expression
        {
            internal Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.method = method;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitSuper(this);
            }
            public Token keyword;
            public Token method;
        }

    }
} // namespace cslox
