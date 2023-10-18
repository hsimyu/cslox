﻿using System;
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

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
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
            return null;
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
    }
}
