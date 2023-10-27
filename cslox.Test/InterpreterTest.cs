﻿using NuGet.Frameworks;
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
        public void Add()
        {
            var script = @"
                var a = 10;
                var b = 1;
                a * b;";

            Assert.AreEqual(10, Convert.ToInt32(Test(script)));
        }
    }
}
