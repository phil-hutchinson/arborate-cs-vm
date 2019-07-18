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
            var functionDefinition = new FunctionDefinition(instructions, inParams, outParams, varCount);
            var machine = new VirtualMachine(functionDefinition);
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

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public void FunctionWithIncorrectReturnArgumentCountThrows(int stackCount, int outParamCount)
        {
            var inst = Enumerable.Repeat(0, stackCount).Select(dummy => new Instruction(BooleanConstantToStack, true)).ToList();
            var outParams = Enumerable.Repeat(VmType.Boolean, outParamCount).ToList();

            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteFunction(inst, outParams: outParams));

            Assert.Equal(InvalidSourceDetail.IncorrectReturnArgumentCount, exception.DetailCode);
        }

        public static IEnumerable<object[]> MemberData_FunctionWithIncorrectReturnArgumentTypeThrows =>
            new List<object[]>
            {
                new object[] {new VmType[] { VmType.Boolean }, new VmType[] { VmType.Integer }},
                new object[] {new VmType[] { VmType.Integer }, new VmType[] { VmType.Boolean }},
                new object[] {new VmType[] { VmType.Boolean, VmType.Boolean }, new VmType[] { VmType.Boolean, VmType.Integer }},
                new object[] {new VmType[] { VmType.Boolean, VmType.Boolean }, new VmType[] { VmType.Integer, VmType.Integer }},
                new object[] {new VmType[] { VmType.Integer, VmType.Boolean }, new VmType[] { VmType.Boolean, VmType.Integer }},
            };

        [Theory]
        [MemberData(nameof(MemberData_FunctionWithIncorrectReturnArgumentTypeThrows))]
        public void FunctionWithIncorrectReturnArgumentTypeThrows(VmType[] outParams, VmType[] stackParams)
        {
            var instructions = new List<Instruction>();
            foreach(var vmType in Enumerable.Reverse(stackParams))
            {
                switch (vmType)
                {
                    case VmType.Boolean:
                        instructions.Add(new Instruction(BooleanConstantToStack, true));
                        break;

                    case VmType.Integer:
                        instructions.Add(new Instruction(IntegerConstantToStack, 100L));
                        break;

                    default:
                        Assert.True(false); // invalid data for test.
                        break;
                }
            }

            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteFunction(instructions, outParams: outParams));

            Assert.Equal(InvalidSourceDetail.IncorrectReturnArgumentType, exception.DetailCode);
        }

        [Fact]
        public void FunctionWithoutReturnTypeThrowsOnVirtualMachineCreation()
        {
            var instructions = new List<Instruction>()
            {
                new Instruction(BooleanConstantToStack, true),
            };

            var functionDefinition = new FunctionDefinition(instructions, new List<VmType>(), new List<VmType>(), 0);
            var exception = Assert.Throws<InvalidSourceException>(() => new VirtualMachine(functionDefinition));

            Assert.Equal(InvalidSourceDetail.FunctionDefinitionMissingReturnValue, exception.DetailCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-3)]
        public void FunctionWithInvalidInstructionCodesThrows(int instructionCode)
        {
            var instructions = new List<Instruction>()
            {
                new Instruction((InstructionCode)instructionCode),
            };
            var functionDefinition = new FunctionDefinition(instructions, new List<VmType>(), new List<VmType>() { VmType.Boolean }, 0);
            var exception = Assert.Throws<InvalidSourceException>(() => new VirtualMachine(functionDefinition));

            Assert.Equal(InvalidSourceDetail.InvalidInstruction, exception.DetailCode);
        }
    }
}
