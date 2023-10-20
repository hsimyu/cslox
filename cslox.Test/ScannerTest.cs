namespace cslox.Test
{
    [TestClass]
    public class ScannerTest
    {
        [TestMethod]
        public void ScanToken()
        {
            var input = "()";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual(TokenType.LEFT_PAREN, tokens[0].type);
            Assert.AreEqual(TokenType.RIGHT_PAREN, tokens[1].type);
        }

        [TestMethod]
        public void TwoOperators()
        {
            var input = "!=";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(TokenType.BANG_EQUAL, tokens[0].type);
        }

        [TestMethod]
        public void LineComment()
        {
            var input = "// aaa+();\n()";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual(TokenType.LEFT_PAREN, tokens[0].type);
            Assert.AreEqual(TokenType.RIGHT_PAREN, tokens[1].type);
        }

        [TestMethod]
        public void BlockComment()
        {
            var input = "/* aaa+(); \n ccc bbb */()";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual(TokenType.LEFT_PAREN, tokens[0].type);
            Assert.AreEqual(TokenType.RIGHT_PAREN, tokens[1].type);
        }

        [TestMethod]
        public void BlockCommentNested()
        {
            var input = "/* aaa+(); \n /*ccc*/ bbb */()";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual(TokenType.LEFT_PAREN, tokens[0].type);
            Assert.AreEqual(TokenType.RIGHT_PAREN, tokens[1].type);
        }

        [TestMethod]
        public void StringLiteral()
        {
            var input = "\"aaaa\"";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(TokenType.STRING, tokens[0].type);
        }

        [TestMethod]
        public void IntegerLiteral()
        {
            var input = "5 + 5";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(TokenType.NUMBER, tokens[0].type);
            Assert.AreEqual(TokenType.PLUS, tokens[1].type);
            Assert.AreEqual(TokenType.NUMBER, tokens[2].type);
        }

        [TestMethod]
        public void FloatLiteral()
        {
            var input = "5.0 + 5.0";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(TokenType.NUMBER, tokens[0].type);
            Assert.AreEqual(TokenType.PLUS, tokens[1].type);
            Assert.AreEqual(TokenType.NUMBER, tokens[2].type);
        }

        [TestMethod]
        public void Identifier()
        {
            var input = "a + 5.0";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(TokenType.IDENTIFIER, tokens[0].type);
            Assert.AreEqual("a", tokens[0].literal);
            Assert.AreEqual(TokenType.PLUS, tokens[1].type);
            Assert.AreEqual(TokenType.NUMBER, tokens[2].type);
        }

        [TestMethod]
        public void Keyword()
        {
            var input = "a and b";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(TokenType.IDENTIFIER, tokens[0].type);
            Assert.AreEqual("a", tokens[0].literal);
            Assert.AreEqual(TokenType.AND, tokens[1].type);
            Assert.AreEqual(TokenType.IDENTIFIER, tokens[2].type);
            Assert.AreEqual("b", tokens[2].literal);
        }

        [TestMethod]
        public void Ternary()
        {
            var input = "1 ? 2 : 3";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(TokenType.NUMBER, tokens[0].type);
            Assert.AreEqual(TokenType.QUESTION, tokens[1].type);
            Assert.AreEqual(TokenType.NUMBER, tokens[2].type);
            Assert.AreEqual(TokenType.COLON, tokens[3].type);
            Assert.AreEqual(TokenType.NUMBER, tokens[4].type);
        }
    }
}