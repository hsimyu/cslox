/* This is auto-generated code. Do not edit. */
namespace cslox
{

    public abstract class Stmt
    {
        public interface IVisitor<R>
        {
            R visitExpressionStmt(ExpressionStmt stmt);
            R visitPrintStmt(PrintStmt stmt);
            R visitVarStmt(VarStmt stmt);
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

        public class VarStmt : Stmt
        {
            internal VarStmt(Token name, Expression initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitVarStmt(this);
            }
            public Token name;
            public Expression initializer;
        }

    }
} // namespace cslox
