/* This is auto-generated code. Do not edit. */
namespace cslox
{

    public abstract class Stmt
    {
        public interface IVisitor<R>
        {
            R visitExpression(Expression stmt);
            R visitPrint(Print stmt);
        }
        public abstract R accept<R>(IVisitor<R> visitor);

        public class Expression : Stmt
        {
            internal Expression(Expression expression)
            {
                this.expression = expression;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitExpression(this);
            }
            public Expression expression;
        }

        public class Print : Stmt
        {
            internal Print(Expression expression)
            {
                this.expression = expression;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitPrint(this);
            }
            public Expression expression;
        }

    }
} // namespace cslox
