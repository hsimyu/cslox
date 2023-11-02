﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace cslox
{
    public class Resolver : Expression.IVisitor<object?>, Stmt.IVisitor<object?>
    {
        Interpreter interpreter;
        Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public object? visitBlockStmt(Stmt.BlockStmt stmt)
        {
            beginScope();
            resolve(stmt.statements);
            endScope();
            return null;
        }

        public void resolve(List<Stmt> stmts)
        {
            foreach (var statement in stmts)
            {
                resolve(statement);
            }
        }

        void resolve(Stmt stmt)
        {
            stmt.accept(this);
        }

        void resolve(Expression expr)
        {
            expr.accept(this);
        }

        void beginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        void endScope()
        {
            scopes.Pop();
        }

        public object? visitVarStmt(Stmt.VarStmt stmt)
        {
            declare(stmt.name);
            if (stmt.initializer != null)
            {
                resolve(stmt.initializer);
            }
            define(stmt.name);
            return null;
        }

        void declare(Token name)
        {
            if (scopes.Count == 0) return;

            var scope = scopes.Peek();
            scope.Add(name.lexeme, false); // bool は not ready yet マーク
        }

        void define(Token name)
        {
            if (scopes.Count == 0) return;

            var scope = scopes.Peek();
            if (scope.ContainsKey(name.lexeme))
            {
                scope[name.lexeme] = true;
            }
            else
            {
                scope.Add(name.lexeme, true); // 初期化完了
            }
        }

        public object? visitVariable(Expression.Variable expr)
        {
            // 変数参照時、現在のスコープ内で ready でない場合は参照できない
            if (scopes.Count != 0 && scopes.Peek().TryGetValue(expr.name.lexeme, out var isReady) && !isReady)
            {
                Program.error(expr.name, "Can't read local variable in its own initializer.");
            }

            resolveLocal(expr, expr.name);
            return null;
        }

        void resolveLocal(Expression expr, Token name)
        {
            // 上から順にスタックを見ていって、スコープ内に定義されていたら解決する
            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes.ElementAt(i).ContainsKey(name.lexeme))
                {
                    // 解決結果をインタプリタに保存する
                    // 第 2 引数はスコープ数、つまり参照しているスコープと定義しているスコープがどのくらい離れているかを表す
                    interpreter.resolve(expr, scopes.Count - 1 - i);
                    return;
                }
            }
        }

        public object? visitAssign(Expression.Assign expr)
        {
            resolve(expr.value); // 右辺を解決
            resolveLocal(expr, expr.name); // 左辺を解決
            return null;
        }

        public object? visitFunctionStmt(Stmt.FunctionStmt stmt)
        {
            declare(stmt.name);
            define(stmt.name);
            resolveFunction(stmt);
            return null;
        }

        void resolveFunction(Stmt.FunctionStmt stmt)
        {
            // スコープの導入と、ブロック内のパラメータの束縛
            beginScope();
            foreach (Token param in stmt.arguments)
            {
                declare(param);
                define(param);
            }
            resolve(stmt.body);
            endScope();
        }

        public object? visitExpressionStmt(Stmt.ExpressionStmt stmt)
        {
            resolve(stmt.expression);
            return null;
        }

        public object? visitIfStmt(Stmt.IfStmt ifStmt)
        {
            resolve(ifStmt.condition);
            resolve(ifStmt.thenStmt);

            if (ifStmt.elseStmt != null)
            {
                resolve(ifStmt.elseStmt);
            }
            return null;
        }

        public object? visitPrintStmt(Stmt.PrintStmt printStmt)
        {
            resolve(printStmt.expression);
            return null;
        }

        public object? visitReturnStmt(Stmt.ReturnStmt returnStmt)
        {
            if (returnStmt.expr != null)
            {
                resolve(returnStmt.expr);
            }
            return null;
        }

        public object? visitWhileStmt(Stmt.WhileStmt whileStmt)
        {
            resolve(whileStmt.condition);
            resolve(whileStmt.body);
            return null;
        }

        public object? visitBinary(Expression.Binary expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }

        public object? visitTernary(Expression.Ternary expr)
        {
            resolve(expr.cond);
            resolve(expr.first);
            resolve(expr.second);
            return null;
        }

        public object? visitCall(Expression.Call call)
        {
            resolve(call.callee);
            foreach (Expression e in call.arguments)
            {
                resolve(e);
            }
            return null;
        }

        public object? visitGrouping(Expression.Grouping expr)
        {
            resolve(expr.exp);
            return null;
        }

        public object? visitLiteral(Expression.Literal expr)
        {
            return null;
        }

        public object? visitLogical(Expression.Logical expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }

        public object? visitUnary(Expression.Unary expr)
        {
            resolve(expr.right);
            return null;
        }
    }
}
