using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    interface Callable
    {
        int arity();
        object? call(Interpreter interpreter, List<object?> arguments);
    }
}
