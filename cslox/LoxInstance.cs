using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class LoxInstance
    {
        LoxClass klass;
        Dictionary<string, object?> fields = new Dictionary<string, object?>();

        internal LoxInstance(LoxClass klass)
        {
            this.klass = klass;
        }

        internal object? get(Token name)
        {
            if (fields.ContainsKey(name.lexeme))
                return fields[name.lexeme];

            var method = klass.findMethod(name.lexeme);
            if (method != null) return method;

            throw new RuntimeError(name, $"Undefined property '{name.lexeme}'.");
        }

        internal void set(Token name, object? value)
        {
            fields[name.lexeme] = value;
        }

        public override string ToString()
        {
            return $"{klass.name} instance";
        }
    }
}
