using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arborate.Runtime.Entity
{
    public class FunctionDefinition
    {
        public IReadOnlyList<Instruction>  Code { get; }
        public IReadOnlyList<VmType> InParams { get; }
        public IReadOnlyList<VmType> OutParams { get; }
        public int VarCount { get; }

        public FunctionDefinition(IEnumerable<Instruction> code, IEnumerable<VmType> inParams, IEnumerable<VmType> outParams, int varCount)
        {
            Code = code.ToList().AsReadOnly();
            InParams = inParams.ToList().AsReadOnly();
            OutParams = outParams.ToList().AsReadOnly();
            VarCount = varCount;
        }
    }
}
