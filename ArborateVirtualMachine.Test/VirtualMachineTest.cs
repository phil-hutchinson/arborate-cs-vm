using ArborateVirtualMachine.Entity;
using static ArborateVirtualMachine.Entity.InstructionCode;
using System;
using System.Collections.Generic;
using Xunit;

namespace ArborateVirtualMachine.Test
{
    public class VirtualMachineTest
    {
        private bool ExecuteBooleanFunction(IEnumerable<Instruction> instructions)
        {
            var definition = new FunctionDefinition(instructions, new List<VmType>(), new List<VmType>(), 0);
            var machine = new VirtualMachine(definition);
            var executionResult = machine.Execute();
            return ((VmBoolean)executionResult).Val;
        }

        [Fact]
        public void SimpleFunctionExecutesCorrectly()
        {
            var definition = new List<Instruction>()
            {
                new Instruction(BooleanConstantToStack, true)
            };
            var actual = ExecuteBooleanFunction(definition);
            Assert.True(actual);
        }
    }
}
