using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class Scanner
    {
        private string source = string.Empty;
        private List<Token> tokens = new List<Token>();

        private int startIndex = 0;
        private int currentIndex = 0;
        private int line = 0;

        internal Scanner(string source)
        {
            this.source = source;
        }

        internal List<Token> scanTokens()
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
                    // addToken(LEFT_PAREN);
                    break;
                default:
                    break;
            }
        }

        char advance()
        {
            return source.ElementAt(currentIndex++);
        }
    }
}
