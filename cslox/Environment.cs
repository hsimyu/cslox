using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public class Environment
    {
        Environment? enclosing;

        Dictionary<string, object?> values = new Dictionary<string, object?>();

        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public void define(string key, object? value)
        {
            values[key] = value;
        }

        public void assign(Token name, object? value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            // 親環境を持つときは、親を探す
            if (enclosing != null)
            {
                enclosing.assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }

        public object? get(Token name)
        {
            if (values.ContainsKey(name.lexeme))
                return values[name.lexeme];

            // 親環境を持つときは、親を探す
            if (enclosing != null) return enclosing.get(name);

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }
    }
}
