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

        public Expression? parse()
        {
            try
            {
                return comma();
            }
            catch (Exception)
            {
                return null;
            }
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
            // expression := equality
            return equality();
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
            var lhs = unary();

            while (match(TokenType.SLASH, TokenType.STAR))
            {
                Token op = previous();
                Expression rhs = unary();
                lhs = new Expression.Binary(lhs, op, rhs);
            }

            return lhs;
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

        void consume(TokenType type, string message)
        {
            if (peek().type != type)
            {
                throw error(peek(), message);
            }

            advance();
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
