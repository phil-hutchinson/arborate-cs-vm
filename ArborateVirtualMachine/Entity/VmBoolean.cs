using System;
using System.Collections.Generic;
using System.Text;

namespace ArborateVirtualMachine.Entity
{
    public class VmBoolean : VmValue
    {
        public bool Val { get; }

        public VmBoolean(bool val)
        {
            Val = val;
        }
    }
}
