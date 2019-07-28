using System;
using System.Collections.Generic;
using System.Text;

namespace Arborate.Runtime.Entity
{
    public class VmBoolean : VmValue
    {
        public override VmType VmType { get { return VmType.Boolean; } }

        public bool Val { get; }

        public VmBoolean(bool val)
        {
            Val = val;
        }
    }
}
