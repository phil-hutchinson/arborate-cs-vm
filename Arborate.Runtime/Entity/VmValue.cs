using System;
using System.Collections.Generic;
using System.Text;

namespace ArborateVirtualMachine.Entity
{
    public abstract class VmValue
    {
        public abstract VmType VmType { get; }
    }
}
