using System;
using System.Collections.Generic;
using System.Linq;

namespace cslox
{
    public class RuntimeError : Exception
    {
        public Token token;

        public RuntimeError(Token token, string message)
            : base(message)
        {
            this.token = token;
        }
    }
}
