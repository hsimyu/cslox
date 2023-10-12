using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cslox
{
    public class Scanner
    {
        private string source = string.Empty;
        private List<Token> tokens = new List<Token>();

        private int startIndex = 0;
        private int currentIndex = 0;
        private int line = 0;

        public Scanner(string source)
        {
            this.source = source;
        }

        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                // 文字列が終わっていなければ、次の字句を先頭にしてスキャンする
                startIndex = currentIndex;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        bool isAtEnd()
        {
            return currentIndex >= source.Length;
        }

        void scanToken()
        {
            char c = advance();

            switch (c)
            {
                case '(':
                    addToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    addToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    addToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    addToken(TokenType.RIGHT_BRACE);
                    break;
                case ',':
                    addToken(TokenType.COMMA);
                    break;
                case '.':
                    addToken(TokenType.DOT);
                    break;
                case '-':
                    addToken(TokenType.MINUS);
                    break;
                case '+':
                    addToken(TokenType.PLUS);
                    break;
                case ';':
                    addToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    addToken(TokenType.STAR);
                    break;
                case '!':
                    addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (match('/'))
                    {
                        // 行コメントなので、行末まで消費する
                        while (peek() != '\n' && !isAtEnd())
                        {
                            advance();
                        }
                    }
                    else
                    {
                        addToken(TokenType.SLASH);
                    }
                    break;
                default:
                    Program.error(line, $"Unexpected character: {c}");
                    break;
            }
        }

        char advance()
        {
            return source.ElementAt(currentIndex++);
        }

        // 次の文字が期待したものならインデックスを進めて true を返す。
        bool match(char expected)
        {
            if (isAtEnd()) return false;

            // advance() 時にすでにインデックスは進んでいる
            if (source.ElementAt(currentIndex) != expected) return false;

            advance();
            return true;
        }

        char peek()
        {
            if (isAtEnd()) return '\0';
            return source.ElementAt(currentIndex);
        }

        void addToken(TokenType type)
        {
            addToken(type, null);
        }

        void addToken(TokenType type, object? literal)
        {
            string text = source.Substring(startIndex, currentIndex - startIndex);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
