using ArborateVirtualMachine.Entity;
using ArborateVirtualMachine.Exception;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static ArborateVirtualMachine.Entity.InstructionCode;

namespace ArborateVirtualMachine.Test.Integer
{
    public class IntegerInsructionTest : BaseTest
    {
        #region InstructionCodes
        [Theory]
        [InlineData(-6L)]
        [InlineData(0L)]
        [InlineData(12L)]
        public void IntegerToStackExecutesCorrectly(long value)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, value)
            };
            var actual = ExecuteIntegerFunction(inst);
            Assert.Equal(value, actual);
        }

        [Theory]
        [InlineData(3L, 3L, true)]
        [InlineData(-5L, 5L, false)]
        [InlineData(0L, 15L, false)]
        [InlineData(-33L, -33L, true)]
        public void IntegerEqualExecutesCorrectly(long val1, long val2, bool expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, val1),
                new Instruction(IntegerConstantToStack, val2),
                new Instruction(IntegerEqual)
            };
            var actual = ExecuteBooleanFunction(inst);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(13L, 3L, true)]
        [InlineData(-5L, -5L, false)]
        [InlineData(0L, 0L, false)]
        [InlineData(-1L, 1L, true)]
        public void IntegerNotEqualExecutesCorrectly(long val1, long val2, bool expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, val1),
                new Instruction(IntegerConstantToStack, val2),
                new Instruction(IntegerNotEqual)
            };
            var actual = ExecuteBooleanFunction(inst);
            Assert.Equal(expected, actual);
        }
        #endregion

        #region ThrownExceptions
        [Theory]
        [InlineData(VmType.Boolean, VmType.Integer, IntegerEqual)]
        [InlineData(VmType.Integer, VmType.Boolean, IntegerEqual)]
        [InlineData(VmType.Boolean, VmType.Boolean, IntegerEqual)]
        [InlineData(VmType.Boolean, VmType.Integer, IntegerNotEqual)]
        [InlineData(VmType.Integer, VmType.Boolean, IntegerNotEqual)]
        [InlineData(VmType.Boolean, VmType.Boolean, IntegerNotEqual)]
        public void BinaryInstructionWithIncorrectTypesOnStackThrows(VmType type1, VmType type2, InstructionCode instructionCode)
        {
            var instructions = new List<Instruction>()
            {
                BuildConstantToStackInstruction(type1),
                BuildConstantToStackInstruction(type2),
                new Instruction(instructionCode)
            };
            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteBooleanFunction(instructions));

            Assert.Equal(InvalidSourceDetail.IncorrectElementTypeOnStack, exception.DetailCode);
        }

        [Theory]
        [InlineData(0, IntegerEqual)]
        [InlineData(1, IntegerEqual)]
        [InlineData(0, IntegerNotEqual)]
        [InlineData(1, IntegerNotEqual)]
        public void BooleanInstructionRequiringMoreElementsThanOnStackThrows(int numberOfValuesOnStack, InstructionCode instructionCode)
        {
            var instructions = new List<Instruction>();

            for (int i = 0; i < numberOfValuesOnStack; i++)
            {
                instructions.Add(new Instruction(IntegerConstantToStack, 0L));
            }

            instructions.Add(new Instruction(instructionCode));

            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteBooleanFunction(instructions));

            Assert.Equal(InvalidSourceDetail.TooFewElementsOnStack, exception.DetailCode);
        }
        #endregion
    }
}
