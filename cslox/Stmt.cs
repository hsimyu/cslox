/* This is auto-generated code. Do not edit. */
namespace cslox
{

    public abstract class Stmt
    {
        public interface IVisitor<R>
        {
            R visitBlockStmt(BlockStmt stmt);
            R visitExpressionStmt(ExpressionStmt stmt);
            R visitPrintStmt(PrintStmt stmt);
            R visitVarStmt(VarStmt stmt);
            R visitIfStmt(IfStmt stmt);
            R visitWhileStmt(WhileStmt stmt);
            R visitFunctionStmt(FunctionStmt stmt);
        }
        public abstract R accept<R>(IVisitor<R> visitor);

        public class BlockStmt : Stmt
        {
            internal BlockStmt(List<Stmt> statements)
            {
                this.statements = statements;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitBlockStmt(this);
            }
            public List<Stmt> statements;
        }

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
            internal VarStmt(Token name, Expression? initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitVarStmt(this);
            }
            public Token name;
            public Expression? initializer;
        }

        public class IfStmt : Stmt
        {
            internal IfStmt(Expression condition, Stmt thenStmt, Stmt? elseStmt)
            {
                this.condition = condition;
                this.thenStmt = thenStmt;
                this.elseStmt = elseStmt;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitIfStmt(this);
            }
            public Expression condition;
            public Stmt thenStmt;
            public Stmt? elseStmt;
        }

        public class WhileStmt : Stmt
        {
            internal WhileStmt(Expression condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitWhileStmt(this);
            }
            public Expression condition;
            public Stmt body;
        }

        public class FunctionStmt : Stmt
        {
            internal FunctionStmt(Token name, List<Token> arguments, List<Stmt> body)
            {
                this.name = name;
                this.arguments = arguments;
                this.body = body;
            }
            public override R accept<R>(IVisitor<R> visitor)
            {
                return visitor.visitFunctionStmt(this);
            }
            public Token name;
            public List<Token> arguments;
            public List<Stmt> body;
        }

    }
} // namespace cslox
