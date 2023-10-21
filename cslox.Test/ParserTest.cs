namespace cslox.Test
{
    [TestClass]
    public class ParserTest
    {
        static AstPrinter printer = new AstPrinter();

        Expression? ParseImpl(string input)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            return parser.parse();
        }

        string Test(string input)
        {
            var result = ParseImpl(input);
            Assert.IsNotNull(result);
            return printer.print(result);
        }

        [TestMethod]
        public void Comma()
        {
            var result = Test("1 + 2, 4 + 3");
            Assert.AreEqual(result, "(, (+ 1 2) (+ 4 3))");
        }

        [TestMethod]
        [DataRow("1 ? 2 : 3", "(? 1 2 3)")]
        [DataRow("1 ? 2 : 3 ? 3 : 4", "(? 1 2 (? 3 3 4))")]
        public void Conditional(string input, string expect)
        {
            var result = Test(input);
            Assert.AreEqual(result, expect);
        }
    }
}