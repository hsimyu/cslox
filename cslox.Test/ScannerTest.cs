namespace cslox.Test
{
    [TestClass]
    public class ScannerTest
    {
        [TestMethod]
        public void Token()
        {
            var input = "()";
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual(TokenType.LEFT_PAREN, tokens[0]);
            Assert.AreEqual(TokenType.RIGHT_PAREN, tokens[1]);
        }
    }
}