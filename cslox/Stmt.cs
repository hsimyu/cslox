/* This is auto-generated code. Do not edit. */
namespace cslox
{

    public abstract class Stmt
    {
        public interface IVisitor<R>
        {
            R visitExpressionStmt(ExpressionStmt stmt);
            R visitPrintStmt(PrintStmt stmt);
        }
        public abstract R accept<R>(IVisitor<R> visitor);

        public class ExpressionStmt : Stmt
        {
            internal ExpressionStmt(Expression expression)
            {
                this.expression = expression;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitExpressionStmt(this);
            }
            public Expression expression;
        }

        public class PrintStmt : Stmt
        {
            internal PrintStmt(Expression expression)
            {
                this.expression = expression;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitPrintStmt(this);
            }
            public Expression expression;
        }

    }
} // namespace cslox
