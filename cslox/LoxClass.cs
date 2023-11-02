using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    internal class LoxClass
    {
        string name;

        internal LoxClass(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
