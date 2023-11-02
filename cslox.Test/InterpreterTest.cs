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

            var resolver = new Resolver(impl);
            resolver.resolve(result);

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
        public void VarAssign()
        {
            var script = @"
                var a = ""aaa"" + ""bbb"";
                print a;";

            Assert.AreEqual("aaabbb", Test(script));
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

        [TestMethod]
        public void If()
        {
            string code;

            code = "var a = 42; var result = 0; if (a == 42) { result = 1; } else { result = 2; } result;";
            Assert.AreEqual(1, Convert.ToInt32(Test(code)));

            code = "var a = 1; var result = 0; if (a == 42) { result = 1; } else { result = 2; } result;";
            Assert.AreEqual(2, Convert.ToInt32(Test(code)));
        }

        [TestMethod]
        public void LogicOr()
        {
            string code;
            code = "print \"hi\" or 2;";
            Assert.AreEqual("hi", Test(code));

            code = "print nil or 2;";
            Assert.AreEqual("2", Test(code));
        }

        [TestMethod]
        public void LogicAnd()
        {
            string code;
            code = "print \"hi\" and 2;";
            Assert.AreEqual("2", Test(code));

            code = "print nil and 2;";
            Assert.AreEqual("nil", Test(code));
        }

        [TestMethod]
        public void While()
        {
            string code;
            code = "var a = 0; while (a < 10) { a = a + 1; } print a;";
            Assert.AreEqual("10", Test(code));
        }

        [TestMethod]
        public void For()
        {
            string code;
            code = "var a = 0; var temp; for (var b = 1; a < 10; b = temp + b) { print a; temp = a; a = b; } print a;";
            Assert.AreEqual("13", Test(code));
        }

        [TestMethod]
        public void FunctionDecl()
        {
            string code;
            code = @"
fun sayHi(first, last) {
print ""Hi, "" + first;
}

sayHi(""Dear"", ""Reader"");
";
            Test(code);
        }

        [TestMethod]
        public void Return()
        {
            string code;
            code = @"
fun fib(n) {
if (n <= 1) return n;
return fib(n-2) + fib(n-1);
}

var result = fib(7);
print result;
";
            Assert.AreEqual("13", Test(code));
        }

        [TestMethod]
        public void Closure()
        {
            string code;
            code = @"
fun makeCounter() {
var i = 0;
fun count() {
  i = i + 1;
  print i;
  return i;
}
return count;
}

var counter = makeCounter();
counter();
counter();
";
            Assert.AreEqual(2.0, Test(code));
        }

        [TestMethod]
        public void Closure2()
        {
            string code;
            code = @"
var result = """";
var a = ""global"";
{
    fun getA() {
      return a;
    }

    result = result + getA();
    var a = ""block"";
    result = result + getA();
}
result;
";
            Assert.AreEqual("globalglobal", Test(code));
        }
    }
}
