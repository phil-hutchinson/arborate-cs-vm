using ArborateVirtualMachine.Entity;
using static ArborateVirtualMachine.Entity.InstructionCode;
using System;
using System.Collections.Generic;
using Xunit;
using ArborateVirtualMachine.Exception;
using System.Linq;

namespace ArborateVirtualMachine.Test
{
    public class VirtualMachineTest
    {
        private bool ExecuteBooleanFunction(IEnumerable<Instruction> instructions)
        {
            var executionResult = ExecuteFunction(instructions, outParams: new List<VmType> { VmType.Boolean });
            return ((VmBoolean)executionResult.Single()).Val;
        }

        private IEnumerable<VmValue> ExecuteFunction(IEnumerable<Instruction> instructions, IEnumerable<VmType> inParams = null, IEnumerable<VmType> outParams = null, int varCount = 0)
        {
            inParams = inParams ?? new List<VmType>();
            outParams = outParams ?? new List<VmType>();
            var inst = new FunctionDefinition(instructions, inParams, outParams, varCount);
            var machine = new VirtualMachine(inst);
            var executionResult = machine.Execute();
            return new List<VmValue>() { executionResult };
            //yield return executionResult; // can remove the yield when vm returns multiple types properly
        }

        [Fact]
        public void SimpleFunctionExecutesCorrectly()
        {
            var inst = new List<Instruction>()
            {
                new Instruction(BooleanConstantToStack, true)
            };
            var actual = ExecuteBooleanFunction(inst);
            Assert.True(actual);
        }

        [Fact]
        public void FunctionWithEmptyReturnStackThrows()
        {
            var inst = new List<Instruction>()
            {
            };
            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteBooleanFunction(inst));
            Assert.Equal(InvalidSourceDetail.IncorrectReturnArgumentCount, exception.DetailCode);
        }

        [Fact]
        public void FunctionWithTooShortReturnStackThrows()
        {
            var inst = new List<Instruction>()
            {
                new Instruction(BooleanConstantToStack, true)
            };
            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteFunction(inst, outParams: new List<VmType>() { VmType.Boolean, VmType.Boolean }));
            Assert.Equal(InvalidSourceDetail.IncorrectReturnArgumentCount, exception.DetailCode);
        }

        [Fact]
        public void FunctionWithTooLongReturnStackThrows()
        {
            var inst = new List<Instruction>()
            {
                new Instruction(BooleanConstantToStack, true),
                new Instruction(BooleanConstantToStack, true)
            };
            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteBooleanFunction(inst));
            Assert.Equal(InvalidSourceDetail.IncorrectReturnArgumentCount, exception.DetailCode);
        }
    }
}
