using ArborateVirtualMachine.Entity;
using static ArborateVirtualMachine.Entity.InstructionCode;
using System;
using System.Collections.Generic;
using Xunit;
using ArborateVirtualMachine.Exception;
using System.Linq;

namespace ArborateVirtualMachine.Test
{
    public class VirtualMachineTest: BaseTest
    {
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

        [Theory]
        [InlineData(BooleanConstantToStack)]
        [InlineData(IntegerConstantToStack)]
        public void InstructionMissingRequiredDataThrows(InstructionCode instructionCode)
        {
            var instructions = new List<Instruction>()
            {
                new Instruction(instructionCode)
            };

            var functionDefinition = new FunctionDefinition(instructions, new List<VmType>(), new List<VmType>() { VmType.Boolean }, 0);
            var exception = Assert.Throws<InvalidSourceException>(() => new VirtualMachine(functionDefinition));

            Assert.Equal(InvalidSourceDetail.MissingInstructionData, exception.DetailCode);
        }

        [Theory]
        [InlineData(BooleanConstantToStack, VmType.Integer)]
        [InlineData(IntegerConstantToStack, VmType.Boolean)]
        public void InstructionRequiringBooleanWithInvalidDataThrows(InstructionCode instructionCode, VmType vmType)
        {

            var instructions = new List<Instruction>();

            switch(vmType)
            {
                case VmType.Integer:
                    instructions.Add(new Instruction(instructionCode, 0L));
                    break;

                case VmType.Boolean:
                    instructions.Add(new Instruction(instructionCode, true));
                    break;

                default:
                    Assert.True(false); // error in test;
                    break;
            }

            var functionDefinition = new FunctionDefinition(instructions, new List<VmType>(), new List<VmType>() { VmType.Boolean }, 0);
            var exception = Assert.Throws<InvalidSourceException>(() => new VirtualMachine(functionDefinition));

            Assert.Equal(InvalidSourceDetail.InvalidInstructionData, exception.DetailCode);
        }

        [Theory]
        [InlineData(BooleanEqual)]
        [InlineData(BooleanNotEqual)]
        [InlineData(BooleanAnd)]    
        [InlineData(BooleanOr)]
        [InlineData(BooleanNot)]
        [InlineData(IntegerEqual)]
        [InlineData(IntegerNotEqual)]
        [InlineData(IntegerAdd)]
        [InlineData(IntegerSubtract)]
        [InlineData(IntegerMultiply)]
        [InlineData(IntegerDivide)]
        [InlineData(IntegerModulus)]
        public void InstructionWithUnnecessaryDataThrows(InstructionCode instructionCode)
        {
            var instructions = new List<Instruction>()
            {
                new Instruction(instructionCode, 0L)
            };

            var functionDefinition = new FunctionDefinition(instructions, new List<VmType>(), new List<VmType>() { VmType.Boolean }, 0);
            var exception = Assert.Throws<InvalidSourceException>(() => new VirtualMachine(functionDefinition));

            Assert.Equal(InvalidSourceDetail.InstructionCodeDoesNotUseData, exception.DetailCode);
        }

    }
}
