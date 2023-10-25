using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public class Environment
    {
        Dictionary<string, object?> values = new Dictionary<string, object?>();

        public void define(string key, object? value)
        {
            values[key] = value;
        }

        public object? get(Token name)
        {
            if (values.ContainsKey(name.lexeme))
                return values[name.lexeme];

            throw new RuntimeError(name, $"Undefined variable '{name.lexeme}'.");
        }
    }
}
