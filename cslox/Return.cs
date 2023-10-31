using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace cslox
{
    internal class Return : Exception
    {
        internal object? value;

        internal Return(object? value)
            : base()
        {
            this.value = value;
        }
    }
}
