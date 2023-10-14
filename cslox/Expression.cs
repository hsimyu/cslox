using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    abstract class Expression
    {
    }

    class Binary : Expression
    {
        Binary(Expression left, Token op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        Expression left;
        Token op;
        Expression right;
    }
}
