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
            var init = findMethod("init");
            if (init == null) return 0;
            return init.arity();
        }

        public object? call(Interpreter interpreter, List<object?> arguments)
        {
            var instance = new LoxInstance(this);
            var init = findMethod("init"); // "init" を初期化関数の標準名とする
            if (init != null)
            {
                init.bind(instance).call(interpreter, arguments);
            }
            return instance;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
