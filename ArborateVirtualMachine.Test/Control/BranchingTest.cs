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
        #endregion

        #region ThrownExceptions
        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(4)]
        public void BranchOutOfRangeThrows(long destinationLineNumber)
        {
            var instructions = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, 1L),
                new Instruction(IntegerConstantToStack, 1L),
                new Instruction(Branch, destinationLineNumber),
                new Instruction(IntegerAdd),
            };

            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteIntegerFunction(instructions));

            Assert.Equal(InvalidSourceDetail.InvalidBranchDestination, exception.DetailCode);
        }
        #endregion
    }
}
