using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public class Interpreter : Expression.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        internal Environment globalEnv = new Environment();
        Environment env;

        // 式と、そこに含まれるローカル変数の解決時の深度の紐づけを保存しておく辞書
        Dictionary<Expression, int> locals = new Dictionary<Expression, int>();

        public Interpreter()
        {
            this.env = globalEnv;

            globalEnv.define("clock", new NativeFunction.Clock());
            globalEnv.define("timespan", new NativeFunction.TimeSpan());
        }

        public object? interpret(List<Stmt> stmts)
        {
            object? evaluated = null;
            try
            {
                foreach(Stmt stmt in stmts)
                {
                    evaluated = execute(stmt);
                }
            }
            catch (RuntimeError re)
            {
                Program.runtimeError(re);
            }
            return evaluated;
        }

        internal object? execute(Stmt stmt)
        {
            return stmt.accept(this);
        }

        internal void executeBlock(List<Stmt> statements, Environment newEnv)
        {
            var prevEnv = env;
            try
            {
                env = newEnv;

                foreach(Stmt stmt in statements)
                {
                    execute(stmt);
                }
            }
            finally
            {
                env = prevEnv;
            }
        }

        internal void resolve(Expression expr, int depth)
        {
            locals.Add(expr, depth);
        }

        public object? visitBlockStmt(Stmt.BlockStmt block)
        {
            executeBlock(block.statements, new Environment(env));
            return null;
        }

        public object? visitClassStmt(Stmt.ClassStmt classStmt)
        {
            env.define(classStmt.name.lexeme, null);
            var klass = new LoxClass(classStmt.name.lexeme);
            env.assign(classStmt.name, klass);
            return null;
        }

        public object? visitIfStmt(Stmt.IfStmt ifStmt)
        {
            if (isTruthy(evaluate(ifStmt.condition)))
            {
                execute(ifStmt.thenStmt);
            }
            else if (ifStmt.elseStmt != null)
            {
                execute(ifStmt.elseStmt);
            }
            return null;
        }

        public object? visitWhileStmt(Stmt.WhileStmt stmt)
        {
            while (isTruthy(evaluate(stmt.condition)))
            {
                execute(stmt.body);
            }
            return null;
        }

        public object? visitFunctionStmt(Stmt.FunctionStmt stmt)
        {
            LoxFunction f = new LoxFunction(stmt, env);
            env.define(stmt.name.lexeme, f);
            return null;
        }

        public object? visitReturnStmt(Stmt.ReturnStmt stmt)
        {
            object? value = null;
            if (stmt.expr != null)
            {
                value = evaluate(stmt.expr);
            }
            throw new Return(value);
        }

        public object? visitExpressionStmt(Stmt.ExpressionStmt stmt)
        {
            return evaluate(stmt.expression);
        }

        public object? visitPrintStmt(Stmt.PrintStmt stmt)
        {
            var value = evaluate(stmt.expression);
            var output = stringify(value);
            Console.WriteLine(output);
            return output; // テスト用
        }

        public object? visitVarStmt(Stmt.VarStmt stmt)
        {
            if (stmt.initializer != null)
            {
                var value = evaluate(stmt.initializer);
                env.define(stmt.name.lexeme, value);
            }
            else
            {
                env.define(stmt.name.lexeme, null);
            }
            return null;
        }

        public string stringify(object? value)
        {
            if (value == null) return "nil";

            if (value is double)
            {
                var text = value.ToString() ?? "";
                return text;
            }

            return value.ToString() ?? "";
        }

        public object? visitAssign(Expression.Assign assign)
        {
            var value = evaluate(assign.value);

            if (locals.TryGetValue(assign, out var distance))
            {
                env.assignAt(distance, assign.name, value);
            }
            else
            {
                globalEnv.assign(assign.name, value);
            }

            return value;
        }

        public object? visitBinary(Expression.Binary expression)
        {
            var left = evaluate(expression.left);
            var right = evaluate(expression.right);

            switch (expression.op.type)
            {
                case TokenType.BANG_EQUAL:
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);
                case TokenType.GREATER:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    checkNumberOperands(expression.op, left, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }

                    throw new RuntimeError(expression.op, "Operands must be two numbers or two strings.");
            }

            return null;
        }

        public object? visitLogical(Expression.Logical expression)
        {
            var left = evaluate(expression.left);
            if (expression.op.type == TokenType.OR)
            {
                // OR -> 左辺が true なら短絡
                if (isTruthy(left)) return left;
                return evaluate(expression.right);
            }
            else
            {
                // AND -> 左辺が false なら短絡
                if (!isTruthy(left)) return left;
                return evaluate(expression.right);
            }
        }

        public object? visitTernary(Expression.Ternary expression)
        {
            var cond = evaluate(expression.cond);
            if (isTruthy(cond))
            {
                return evaluate(expression.first);
            }
            else
            {
                return evaluate(expression.second);
            }
        }

        public object? visitCall(Expression.Call expression)
        {
            var callee = evaluate(expression.callee);

            List<object?> args = new List<object?>();
            foreach (var arg in expression.arguments)
            {
                args.Add(evaluate(arg));
            }

            if (callee is not Callable)
            {
                throw new RuntimeError(expression.paren, "Can only cal functions and classes.");
            }

            Callable f = (Callable)callee;

            // 引数の個数チェック
            if (args.Count != f.arity())
            {
                throw new RuntimeError(expression.paren, $"Expected {f.arity()} arguments but got {args.Count}.");
            }

            return f.call(this, args);
        }


        public object? visitGrouping(Expression.Grouping expression)
        {
            return evaluate(expression.exp);
        }

        public object? visitLiteral(Expression.Literal expression)
        {
            return expression.value;
        }

        public object? visitUnary(Expression.Unary expression)
        {
            object? right = evaluate(expression.right);

            switch (expression.op.type)
            {
                case TokenType.BANG:
                    return !isTruthy(right);
                case TokenType.MINUS:
                    checkNumberOperand(expression.op, expression.right);
                    return -(double)(right ?? 0.0);
            }

            return null;
        }

        public object? visitVariable(Expression.Variable variable)
        {
            return lookUpVariable(variable.name, variable);
        }

        object? lookUpVariable(Token name, Expression expr)
        {
            if (locals.TryGetValue(expr, out var distance))
            {
                return env.getAt(distance, name);
            }
            else
            {
                return globalEnv.get(name);
            }
        }

        object? evaluate(Expression expr)
        {
            return expr.accept(this);
        }

        bool isTruthy(object? value)
        {
            if (value == null) return false;
            if (value is bool) return (bool)value;

            // bool 型でなければ全て true
            return true;
        }

        bool isEqual(object? lhs, object? rhs)
        {
            if (lhs == null && rhs == null) return true;
            if (lhs == null) return false;
            return lhs.Equals(rhs);
        }

        void checkNumberOperand(Token op, object? operand)
        {
            if (operand is double)
            {
                return;
            }
            throw new RuntimeError(op, "Operand must be a number");
        }

        void checkNumberOperands(Token op, object? lhs, object? rhs)
        {
            if ((lhs is double) && (rhs is double))
            {
                return;
            }
            throw new RuntimeError(op, "Operand must be numbers");
        }
    }
}
