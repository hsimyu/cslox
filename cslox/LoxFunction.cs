using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class LoxFunction : Callable
    {
        Stmt.FunctionStmt declaration;

        internal LoxFunction(Stmt.FunctionStmt declaration)
        {
            this.declaration = declaration;
        }

        public int arity()
        {
            return declaration.arguments.Count;
        }

        public object? call(Interpreter interpreter, List<object?> arguments)
        {
            var environment = new Environment(interpreter.globalEnv);
            for (int i = 0; i < declaration.arguments.Count; i++)
            {
                // 仮引数に対応する値を環境に束縛する
                environment.define(declaration.arguments[i].lexeme, arguments[i]);
            }

            try
            {
                interpreter.executeBlock(declaration.body, environment); ;
            }
            catch (Return r)
            {
                // return の実行を例外として捕捉する
                return r.value;
            }

            // return がなければ nil を返す
            return null;
        }

        public override string ToString()
        {
            return $"<fn {declaration.name.lexeme}>";
        }
    }
}
