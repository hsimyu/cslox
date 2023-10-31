using System;

namespace cslox
{
    internal class NativeFunction
    {
        internal class Clock : Callable
        {
            public int arity()
            {
                return 0;
            }

            public object? call(Interpreter interpreter, List<object?> arguments)
            {
                return Convert.ToDouble(DateTime.Now.Ticks);
            }

            public override string ToString()
            {
                return "<native fn>";
            }
        }

        internal class TimeSpan : Callable
        {
            public int arity()
            {
                return 1;
            }

            public object? call(Interpreter interpreter, List<object?> arguments)
            {
                long tick = Convert.ToInt64((double)(arguments[0] ?? 0));
                return new System.TimeSpan(tick).TotalMilliseconds;
            }

            public override string ToString()
            {
                return "<native fn>";
            }
        }
    }
}
