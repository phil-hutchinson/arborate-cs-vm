using ArborateVirtualMachine.Entity;
using ArborateVirtualMachine.Exception;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static ArborateVirtualMachine.Entity.InstructionCode;

namespace ArborateVirtualMachine.Test.Control
{
    public class BranchingTest: BaseTest
    {
        #region InstructionCodes
        [Theory]
        [InlineData(4L, 2310L)]
        [InlineData(6L, 462L)]
        [InlineData(8L, 66L)]
        public void BranchExecutesCorrectly(long branchTo, long expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, 2L),
                new Instruction(IntegerConstantToStack, 3L),
                new Instruction(IntegerMultiply),
                new Instruction(Branch, branchTo),
                new Instruction(IntegerConstantToStack, 5L),
                new Instruction(IntegerMultiply),
                new Instruction(IntegerConstantToStack, 7L),
                new Instruction(IntegerMultiply),
                new Instruction(IntegerConstantToStack, 11L),
                new Instruction(IntegerMultiply),
            };

            var actual = ExecuteIntegerFunction(inst);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(false, 30L)]
        [InlineData(true, 10L)]
        public void BranchTrueExecutesCorrectly(bool checkValue, long expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, 2L),
                new Instruction(BooleanConstantToStack, checkValue),
                new Instruction(BranchTrue, 5L),
                new Instruction(IntegerConstantToStack, 3L),
                new Instruction(IntegerMultiply),
                new Instruction(IntegerConstantToStack, 5L),
                new Instruction(IntegerMultiply),
            };

            var actual = ExecuteIntegerFunction(inst);
            Assert.Equal(expected, actual);
        }
        #endregion

        #region ThrownExceptions
        [Theory]
        [InlineData(VmType.Integer, BranchTrue)]
        public void ConditionalBranchingWithIncorrectTypesOnStackThrows(VmType type, InstructionCode instructionCode)
        {
            var instructions = new List<Instruction>()
                {
                    BuildConstantToStackInstruction(type),
                    new Instruction(instructionCode, 0L)
                };
            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteBooleanFunction(instructions));

            Assert.Equal(InvalidSourceDetail.IncorrectElementTypeOnStack, exception.DetailCode);
        }

        [Theory]
        [InlineData(0, BranchTrue)]
        public void ConditionalBranchingRequiringMoreElementsThanOnStackThrows(int numberOfValuesOnStack, InstructionCode instructionCode)
        {
            var instructions = new List<Instruction>()
            {
                new Instruction(Branch, 1L)
            };

            for (int i = 0; i < numberOfValuesOnStack; i++)
            {
                instructions.Add(new Instruction(BooleanConstantToStack, false));
            }

            instructions.Add(new Instruction(instructionCode, 0L));

            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteBooleanFunction(instructions));

            Assert.Equal(InvalidSourceDetail.TooFewElementsOnStack, exception.DetailCode);
        }

        [Theory]
        [InlineData(Branch, -1)]
        [InlineData(Branch, 2)]
        [InlineData(Branch, 4)]
        [InlineData(BranchTrue, -1)]
        [InlineData(BranchTrue, 2)]
        [InlineData(BranchTrue, 4)]
        public void BranchingOutOfRangeThrows(InstructionCode instructionCode, long destinationLineNumber)
        {
            var instructions = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, 1L),
                new Instruction(IntegerConstantToStack, 1L),
                new Instruction(instructionCode, destinationLineNumber),
                new Instruction(IntegerAdd),
            };

            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteIntegerFunction(instructions));

            Assert.Equal(InvalidSourceDetail.InvalidBranchDestination, exception.DetailCode);
        }
        #endregion
    }
}
