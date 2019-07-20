using ArborateVirtualMachine.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using static ArborateVirtualMachine.Entity.InstructionCode;

namespace ArborateVirtualMachine.Test
{
    public abstract class BaseTest
    {
        protected IEnumerable<VmValue> ExecuteFunction(IEnumerable<Instruction> instructions, IEnumerable<VmType> inParams = null, IEnumerable<VmType> outParams = null, int varCount = 0)
        {
            inParams = inParams ?? new List<VmType>();
            outParams = outParams ?? new List<VmType>();
            var functionDefinition = new FunctionDefinition(instructions, inParams, outParams, varCount);
            var machine = new VirtualMachine(functionDefinition);
            var executionResult = machine.Execute();
            return new List<VmValue>() { executionResult };
            //yield return executionResult; // can remove the yield when vm returns multiple types properly
        }

        protected bool ExecuteBooleanFunction(IEnumerable<Instruction> instructions)
        {
            var executionResult = ExecuteFunction(instructions, outParams: new List<VmType> { VmType.Boolean });
            return ((VmBoolean)executionResult.Single()).Val;
        }

        protected Instruction BuildConstantToStackInstruction(VmType dataType)
        {
            switch(dataType)
            {
                case VmType.Boolean:
                    return new Instruction(BooleanConstantToStack, true);

                case VmType.Integer:
                    return new Instruction(IntegerConstantToStack, 0L);

                default:
                    Assert.True(false);
                    throw new System.Exception("Unrecognized Type");
            }
        }
    }
}
