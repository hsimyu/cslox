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
                if (match(TokenType.VAR)) return varDeclaration();
                return statement();
            }
            catch (ParseError e)
            {
                synchronize();
                return null;
            }
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

        Stmt statement()
        {
            if (match(TokenType.IF)) return ifStatement();
            if (match(TokenType.WHILE)) return whileStatement();
            if (match(TokenType.PRINT)) return printStatement();
            if (match(TokenType.LEFT_BRACE)) return blockStatement();
            return expressionStatement();
        }

        Stmt blockStatement()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            {
                statements.Add(declaration());
            }
            consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return new Stmt.BlockStmt(statements);
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
            // assignment := IDENTIFIER "=" assignment | logic_or
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
            // unary := ( "!" | "-" ) unary | primary
            if (match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = previous();
                Expression rhs = unary();
                return new Expression.Unary(op, rhs);
            }

            return primary();
        }

        Expression primary()
        {
            // primary := NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")"
            if (match(TokenType.FALSE)) return new Expression.Literal(false);
            if (match(TokenType.TRUE)) return new Expression.Literal(true);
            if (match(TokenType.NIL)) return new Expression.Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expression.Literal(previous().literal);
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
