using System;
using System.Collections.Generic;
using System.Text;

namespace Arborate.Runtime.Entity
{
    public class VmInteger : VmValue
    {
        public override VmType VmType { get { return VmType.Integer; } }

        public long Val { get; }

        public VmInteger(long val)
        {
            Val = val;
        }
    }
}
