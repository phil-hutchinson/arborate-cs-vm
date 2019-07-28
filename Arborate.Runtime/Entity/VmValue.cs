using System;
using System.Collections.Generic;
using System.Text;

namespace Arborate.Runtime.Entity
{
    public abstract class VmValue
    {
        public abstract VmType VmType { get; }
    }
}
