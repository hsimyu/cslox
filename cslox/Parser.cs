using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public class Parser
    {
        List<Token> tokens = new List<Token>();
        int currentIndex = 0;

        class ParseError : Exception { }

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> parse()
        {
            List<Stmt> stmts = new List<Stmt>();

            while (!isAtEnd())
            {
                stmts.Add(declaration());
            }

            return stmts;
        }

        Stmt? declaration()
        {
            try
            {
                if (match(TokenType.CLASS)) return classDeclaration();
                if (match(TokenType.FUN)) return function("function");
                if (match(TokenType.VAR)) return varDeclaration();
                return statement();
            }
            catch (ParseError e)
            {
                Console.Error.WriteLine(e.Message);
                synchronize();
                return null;
            }
        }

        Stmt classDeclaration()
        {
            // classDecl := "class" IDENTIFIER "{" function* "}"
            Token name = consume(TokenType.IDENTIFIER, "Expect variable name after 'class'.");
            consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");

            var methods = new List<Stmt.FunctionStmt>();
            while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            {
                methods.Add(function("method"));
            }

            consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");
            return new Stmt.ClassStmt(name, methods);
        }

        Stmt varDeclaration()
        {
            Token name = consume(TokenType.IDENTIFIER, "Expect variable name after 'var'.");
            Expression? initializer = null;
            if (match(TokenType.EQUAL))
            {
                initializer = expression();
            }
            consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.VarStmt(name, initializer);
        }

        Stmt.FunctionStmt function(string kind)
        {
            // funDecl := "fun" function
            // function := IDENTIFIER "(" parameters? ")" block
            // parameters := IDENTIFIER ( "," IDENTIFIER )*
            Token name = consume(TokenType.IDENTIFIER, $"Expect {kind} name after 'fun'.");
            consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");

            List<Token> arguments = new List<Token>();

            if (!check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 parameters.");
                    }
                    arguments.Add(consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (match(TokenType.COMMA));
            }
            consume(TokenType.RIGHT_PAREN, $"Expect ')' after parameters.");

            consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body");
            var body = block();
            return new Stmt.FunctionStmt(name, arguments, body);
        }

        Stmt statement()
        {
            if (match(TokenType.FOR)) return forStatement();
            if (match(TokenType.IF)) return ifStatement();
            if (match(TokenType.WHILE)) return whileStatement();
            if (match(TokenType.PRINT)) return printStatement();
            if (match(TokenType.RETURN)) return returnStatement();
            if (match(TokenType.LEFT_BRACE)) return new Stmt.BlockStmt(block());
            return expressionStatement();
        }

        List<Stmt> block()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            {
                statements.Add(declaration());
            }
            consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        Stmt forStatement()
        {
            // forStmt := "for" "(" statement? ";" expression? ";" expression? ")" statement
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            Stmt? initializer;
            if (match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (match(TokenType.VAR))
            {
                initializer = varDeclaration();
            }
            else
            {
                initializer = expressionStatement();
            }

            Expression? condition = null;
            if (!check(TokenType.SEMICOLON))
            {
                condition = expression();
            }
            consume(TokenType.SEMICOLON, "Expect ';' after loop conditoin");

            Expression? increment = null;
            if (!check(TokenType.SEMICOLON))
            {
                increment = expression();
            }
            consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

            var body = statement();

            // for 文のセマンティクスを while 文で表現する
            if (increment != null)
            {
                body = new Stmt.BlockStmt(new List<Stmt> { body, new Stmt.ExpressionStmt(increment) });
            }

            if (condition == null) condition = new Expression.Literal(true);
            body = new Stmt.WhileStmt(condition, body);

            if (initializer != null)
            {
                body = new Stmt.BlockStmt(new List<Stmt> { initializer, body });
            }

            return body;
        }

        Stmt ifStatement()
        {
            // ifStmt := "if" "(" expression ")" statement ( "else" statement )?
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expression cond = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenStmt = statement();
            Stmt? elseStmt = null;
            if (match(TokenType.ELSE))
            {
                elseStmt = statement();
            }

            return new Stmt.IfStmt(cond, thenStmt, elseStmt);
        }

        Stmt whileStatement()
        {
            // whileStmt := "while" "(" expression ")" statement
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            Expression cond = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after while condition.");

            Stmt body = statement();
            return new Stmt.WhileStmt(cond, body);
        }

        Stmt printStatement()
        {
            var value = comma();
            consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.PrintStmt(value);
        }

        Stmt returnStatement()
        {
            // return := "return" expression? ";"
            Token keyword = previous();
            Expression? expr = null;

            if (!check(TokenType.SEMICOLON))
            {
                expr = comma();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt.ReturnStmt(keyword, expr);
        }

        Stmt expressionStatement()
        {
            var value = comma();
            consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.ExpressionStmt(value);
        }

        Expression comma()
        {
            // comma := conditional ("," conditional)*
            var lhs = conditional();

            while (match(TokenType.COMMA))
            {
                Token op = previous();
                Expression rhs = conditional();
                lhs = new Expression.Binary(lhs, op, rhs);
            }

            return lhs;
        }

        Expression conditional()
        {
            // conditional := expression ("?" conditional ":" conditional)
            // 右結合で、式内の一番左の ? が最初に消費される
            // 左結合だと無限再帰してしまう
            var cond = expression();

            if (match(TokenType.QUESTION))
            {
                Token op = previous();
                Expression first = conditional();
                consume(TokenType.COLON, "Expected ':'.");
                Expression second = conditional();
                return new Expression.Ternary(cond, op, first, second);
            }

            return cond;
        }

        Expression expression()
        {
            // expression := assignment
            return assignment();
        }

        Expression assignment()
        {
            // assignment := (call ".")? IDENTIFIER "=" assignment | logic_or
            var expr = logic_or();

            // NOTE:
            // 先頭のトークンが IDENTIFIER -> EQUAL かどうかで代入式を判別することはできない
            // 例えば obj.foo = 42 とか getAddress() = 1 のような記述が行われたとき、トークンの並びはそれとは変わってくる
            // そのため、まず式として左辺を解釈したあと、区切りとなる位置で "=" が来ているかどうかで代入式を識別する
            if (match(TokenType.EQUAL))
            {
                var equals = previous();
                Expression rhs = assignment();

                if (expr is Expression.Variable)
                {
                    // 変数式だった場合、右辺値ではなくて左辺値として取り扱う
                    Token name = ((Expression.Variable)expr).name;
                    return new Expression.Assign(name, rhs);
                }
                else if (expr is Expression.Get)
                {
                    // オブジェクトのゲッターだったとき、左辺値つまりセッターとして取り扱う
                    var get = (Expression.Get)expr;
                    return new Expression.Set(get.obj, get.name, rhs);
                }

                // ここに到達するのは 1 = ... のような記述をしているとき
                error(equals, "Invalid assignment target.");
            }
            return expr;
        }

        Expression logic_or()
        {
            // logic_or := logic_and ("or" logic_and)*
            var lhs = logic_and();

            while (match(TokenType.OR))
            {
                Token op = previous();
                var rhs = logic_and();
                lhs = new Expression.Logical(lhs, op, rhs);
            }

            return lhs;
        }

        Expression logic_and()
        {
            // logic_and = equality ("and" equality)*
            var lhs = equality();

            while (match(TokenType.AND))
            {
                Token op = previous();
                var rhs = equality();
                lhs = new Expression.Logical(lhs, op, rhs);
            }

            return lhs;
        }

        Expression equality()
        {
            // equality := comparison ( ("!=" | "==") comparison )*
            var lhs = comparison();

            while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = previous();
                Expression rhs = comparison();
                lhs = new Expression.Binary(lhs, op, rhs);
            }

            return lhs;
        }

        Expression comparison()
        {
            // comparison := term ( (">" | ">=" | "<" | "<=" ) term )*
            var lhs = term();

            while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = previous();
                Expression rhs = term();
                lhs = new Expression.Binary(lhs, op, rhs);
            }

            return lhs;
        }

        Expression term()
        {
            // term := factor ( ("-" | "+" ) factor )*
            var lhs = factor();

            while (match(TokenType.MINUS, TokenType.PLUS))
            {
                Token op = previous();
                Expression rhs = factor();
                lhs = new Expression.Binary(lhs, op, rhs);
            }

            return lhs;
        }

        Expression factor()
        {
            // factor := unary ( ("/" | "*" ) unary )*
            var lhs = binaryErrorOrUnary();

            while (match(TokenType.SLASH, TokenType.STAR))
            {
                Token op = previous();
                Expression rhs = binaryErrorOrUnary();
                lhs = new Expression.Binary(lhs, op, rhs);
            }

            return lhs;
        }

        // エラーチェック規則
        Expression binaryErrorOrUnary()
        {
            // binaryErrorOrUnary := "+" | unary
            if (match(
                TokenType.PLUS, 
                TokenType.EQUAL,
                TokenType.EQUAL_EQUAL,
                TokenType.BANG_EQUAL,
                TokenType.SLASH,
                TokenType.STAR,
                TokenType.GREATER,
                TokenType.GREATER_EQUAL, 
                TokenType.LESS,
                TokenType.LESS_EQUAL))
            { 
                throw error(previous(), "Missing left hand side.");
            }

            return unary();
        }

        Expression unary()
        {
            // unary := ( "!" | "-" ) unary | call
            if (match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = previous();
                Expression rhs = unary();
                return new Expression.Unary(op, rhs);
            }

            return call();
        }

        Expression call()
        {
            // call := primary ( "(" arguments? ")" | "." IDENTIFIER )*
            var expr = primary();

            while (true)
            {
                // "(" に出会うたびに再帰的に関数コールと認識する
                if (match(TokenType.LEFT_PAREN))
                {
                    expr = finishCall(expr);
                }
                else if (match(TokenType.DOT))
                {
                    // クラスのプロパティアクセス
                    Token name = consume(TokenType.IDENTIFIER, "Expect property name after '.'");
                    expr = new Expression.Get(expr, name);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        Expression finishCall(Expression callee)
        {
            List<Expression> args = new List<Expression>();
            if (!check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (args.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 arguments.");
                    }
                    args.Add(expression());
                } while (match(TokenType.COMMA));
            }

            Token paren = consume(TokenType.RIGHT_PAREN, "Expect ')' after function arguments.");
            return new Expression.Call(callee, paren, args);
        }

        Expression primary()
        {
            // primary := NUMBER | STRING | "true" | "false" | "nil" | IDENTIFIER | "this" |  "(" expression ")"
            if (match(TokenType.FALSE)) return new Expression.Literal(false);
            if (match(TokenType.TRUE)) return new Expression.Literal(true);
            if (match(TokenType.NIL)) return new Expression.Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expression.Literal(previous().literal);
            }

            if (match(TokenType.THIS))
            {
                return new Expression.This(previous());
            }

            if (match(TokenType.IDENTIFIER))
            {
                return new Expression.Variable(previous());
            }
            
            if (match(TokenType.LEFT_PAREN))
            {
                Expression expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expression.Grouping(expr);
            }

            throw error(peek(), "Expect expression.");
        }

        bool match(params TokenType[] types)
        {
            foreach (var t in types)
            {
                if (check(t))
                {
                    advance();
                    return true;
                }
            }
            return false;
        }

        Token advance()
        {
            if (!isAtEnd()) currentIndex++;
            return previous();
        }

        Token consume(TokenType type, string message)
        {
            if (peek().type != type)
            {
                throw error(peek(), message);
            }

            return advance();
        }

        bool check(TokenType type)
        {
            if (isAtEnd()) return false;
            return peek().type == type;
        }

        bool isAtEnd()
        {
            return peek().type == TokenType.EOF;
        }

        Token peek()
        {
            return tokens[currentIndex];
        }

        Token previous()
        {
            return tokens[currentIndex - 1];
        }

        void synchronize()
        {
            advance();
            while (!isAtEnd())
            {
                // セミコロンを見つけていた場合は、そこが同期ポイント
                if (previous().type == TokenType.SEMICOLON) return;

                switch (peek().type)
                {
                    // その他の同期ポイント一覧
                    case TokenType.CLASS:
                    case TokenType.FOR:
                    case TokenType.FUN:
                    case TokenType.IF:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                    case TokenType.VAR:
                    case TokenType.WHILE:
                        return;
                }
                advance();
            }
            // EOF に到達した
            advance();
        }

        ParseError error(Token token, string message)
        {
            Program.error(token, message);
            return new ParseError();
        }
    }
}
