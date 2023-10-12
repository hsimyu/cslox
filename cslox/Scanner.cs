using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    addToken(TokenType.LEFT_PAREN, null);
                    break;
                case ')':
                    addToken(TokenType.RIGHT_PAREN, null);
                    break;
                case '{':
                    addToken(TokenType.LEFT_BRACE, null);
                    break;
                case '}':
                    addToken(TokenType.RIGHT_BRACE, null);
                    break;
                case ',':
                    addToken(TokenType.COMMA, null);
                    break;
                case '.':
                    addToken(TokenType.DOT, null);
                    break;
                case '-':
                    addToken(TokenType.MINUS, null);
                    break;
                case '+':
                    addToken(TokenType.PLUS, null);
                    break;
                case ';':
                    addToken(TokenType.SEMICOLON, null);
                    break;
                case '*':
                    addToken(TokenType.STAR, null);
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

        void addToken(TokenType type, object? literal)
        {
            string text = source.Substring(startIndex, currentIndex - startIndex);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
