namespace cslox.Test
{
    [TestClass]
    public class ParserTest
    {
        static AstPrinter printer = new AstPrinter();

        List<Stmt> ParseImpl(string input)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            return parser.parse();
        }

        string Test(string input)
        {
            var result = ParseImpl(input).First();
            Assert.IsNotNull(result);
            var expression = (Stmt.ExpressionStmt)result;
            return printer.print(expression.expression);
        }

        [TestMethod]
        public void Comma()
        {
            var result = Test("1 + 2, 4 + 3;");
            Assert.AreEqual(result, "(, (+ 1 2) (+ 4 3))");
        }

        [TestMethod]
        [DataRow("1 ? 2 : 3;", "(? 1 2 3)")]
        [DataRow("1 ? 2 : 3 ? 3 : 4;", "(? 1 2 (? 3 3 4))")]
        public void Conditional(string input, string expect)
        {
            var result = Test(input);
            Assert.AreEqual(result, expect);
        }
    }
}