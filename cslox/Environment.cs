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

        public void assignAt(int distance, Token name, object? value)
        {
            ancestor(distance).values.Add(name.lexeme, value);
        }

        public object? get(Token name)
        {
            if (values.ContainsKey(name.lexeme))
                return values[name.lexeme];

            // 親環境を持つときは、親を探す
            if (enclosing != null) return enclosing.get(name);

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }

        public object? getAt(int distance, string name)
        {
            return ancestor(distance).values[name];
        }

        Environment ancestor(int distance)
        {
            Environment result = this;

            for (int i = 0; i < distance; i++)
            {
                result = result.enclosing;
            }

            return result;
        }
    }
}
