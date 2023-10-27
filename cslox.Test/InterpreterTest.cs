using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox.Test
{
    [TestClass]
    public class InterpreterTest
    {
        static Interpreter impl = new Interpreter();

        List<Stmt> ParseImpl(string input)
        {
            var scanner = new Scanner(input);
            var tokens = scanner.scanTokens();
            var parser = new Parser(tokens);
            return parser.parse();
        }

        object? Test(string input)
        {
            var result = ParseImpl(input);
            Assert.IsNotNull(result);
            return impl.interpret(result);
        }

        [TestMethod]
        public void VarMul()
        {
            var script = @"
                var a = 10;
                var b = 1;
                a * b;";

            Assert.AreEqual(10, Convert.ToInt32(Test(script)));
        }

        [TestMethod]
        public void PrintVar()
        {
            var script = @"
                var a = 10 + 23 + 9;
                print a;";

            Assert.AreEqual("42", Test(script));
        }

        [TestMethod]
        public void ScopedEnvironment()
        {
            string code = "";
            code += "var a = \"global a\";";
            code += "var b = \"global b\";";
            code += "var c = \"global c\";";
            code += "{";
            code += "    var a = 10;";
            code += "    var b = 20;";
            code += "    c = a + b;";
            code += "}";
            code += "print c;";
            Assert.AreEqual("30", Test(code));
            code += "print a;";
            Assert.AreEqual("global a", Test(code));
        }
    }
}
