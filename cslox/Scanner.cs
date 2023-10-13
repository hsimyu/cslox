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

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE },
        };

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
                case '"':
                    // 文字列リテラル
                    consumeString();
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // 空白を無視する
                    break;
                case '\n':
                    line++;
                    break;
                default:
                    if (isDigit(c))
                    {
                        consumeNumber();
                    }
                    else if (isAlpha(c))
                    {
                        consumeIdentifier();
                    }
                    else
                    {
                        Program.error(line, $"Unexpected character: {c}");
                    }
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

        bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == '_');
        }

        bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);
        }

        void consumeString()
        {
            // 次の " の手前まで消費
            while (peek() != '"' && !isAtEnd())
            {
                if (peek() == '\n') line++;
                advance();
            }

            if (isAtEnd())
            {
                Program.error(line, "Unterminated string.");
                return;
            }

            // 終端の " を消費
            advance();

            // "" で囲まれた内部を文字列リテラルとする
            string value = source.Substring(startIndex + 1, currentIndex - startIndex - 2);
            addToken(TokenType.STRING, value);
        }

        void consumeNumber()
        {
            while (isDigit(peek()))
            {
                advance();
            }

            // 小数部対応
            if (peek() == '.' && isDigit(peekNext()))
            {
                // 小数点を消費
                advance();

                while (isDigit(peek()))
                {
                    advance();
                }
            }

            string value = source.Substring(startIndex, currentIndex - startIndex);
            addToken(TokenType.NUMBER, double.Parse(value));
        }

        void consumeIdentifier()
        {
            while (isAlphaNumeric(peek()))
            {
                advance();
            }

            // キーワードかどうかを判定し、そうでなければ識別子とする
            string value = source.Substring(startIndex, currentIndex - startIndex);
            if (Keywords.TryGetValue(value, out var keyword))
            {
                addToken(keyword);
            }
            else
            {
                addToken(TokenType.IDENTIFIER, value);
            }
        }

        char peek()
        {
            if (isAtEnd()) return '\0';
            return source.ElementAt(currentIndex);
        }

        char peekNext()
        {
            if (currentIndex + 1 >= source.Length) return '\0';
            return source.ElementAt(currentIndex + 1);
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
