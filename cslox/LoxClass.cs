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
        Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();

        internal LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.methods = methods;
        }

        internal LoxFunction? findMethod(string name)
        {
            if (methods.ContainsKey(name))
                return methods[name];

            return null;
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
