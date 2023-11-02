using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class LoxClass : LoxCallable
    {
        internal string name;

        internal LoxClass(string name)
        {
            this.name = name;
        }

        public int arity()
        {
            return 0;
        }

        public object? call(Interpreter interpreter, List<object?> arguments)
        {
            var instance = new LoxInstance(this);
            return instance;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
