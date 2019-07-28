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

        [Theory]
        [InlineData(12L, 5L, 17L)]
        [InlineData(0L, -6L, -6L)]
        [InlineData(4L, -19L, -15L)]
        [InlineData(-10L, 10L, 0L)]
        public void IntegerAddExecutesCorrectly(long val1, long val2, long expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, val1),
                new Instruction(IntegerConstantToStack, val2),
                new Instruction(IntegerAdd)
            };
            var actual = ExecuteIntegerFunction(inst);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(8L, 5L, 3L)]
        [InlineData(0L, -9L, 9L)]
        [InlineData(-15L, 25L, -40L)]
        [InlineData(-15L, -25L, 10L)]
        [InlineData(10L, 10L, 0L)]
        [InlineData(6L, 0L, 6L)]
        public void IntegerSubtractExecutesCorrectly(long val1, long val2, long expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, val1),
                new Instruction(IntegerConstantToStack, val2),
                new Instruction(IntegerSubtract)
            };
            var actual = ExecuteIntegerFunction(inst);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(7L, 12L, 84L)]
        [InlineData(33L, -6L, -198L)]
        [InlineData(-7L, 7L, -49L)]
        [InlineData(-100L, -100L, 10000L)]
        [InlineData(0L, -15L, 0L)]
        [InlineData(10L, 0L, 0L)]
        public void IntegerMultiplyExecutesCorrectly(long val1, long val2, long expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, val1),
                new Instruction(IntegerConstantToStack, val2),
                new Instruction(IntegerMultiply)
            };
            var actual = ExecuteIntegerFunction(inst);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(26L, 2L, 13L)]
        [InlineData(33L, -11L, -3L)]
        [InlineData(-200L, 10L, -20L)]
        [InlineData(-123L, -123L, 1L)]
        [InlineData(41L, 5L, 8L)]
        [InlineData(13L, -5L, -2L)]
        [InlineData(-59L, 10L, -5L)]
        [InlineData(-61L, -7L, 8L)]
        [InlineData(-6L, 23L, 0L)]
        [InlineData(0L, -1L, 0L)]
        [InlineData(10L, 0L, 0L)]
        [InlineData(0L, 0L, 0L)]
        public void IntegerDivideExecutesCorrectly(long val1, long val2, long expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, val1),
                new Instruction(IntegerConstantToStack, val2),
                new Instruction(IntegerDivide)
            };
            var actual = ExecuteIntegerFunction(inst);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(26L, 2L, 0L)]
        [InlineData(33L, -11L, 0L)]
        [InlineData(-200L, 10L, 0L)]
        [InlineData(-123L, -123L, 0L)]
        [InlineData(41L, 5L, 1L)]
        [InlineData(13L, -5L, 3L)]
        [InlineData(-59L, 10L, -9L)]
        [InlineData(-61L, -7L, -5L)]
        [InlineData(-6L, 23L, -6L)]
        [InlineData(0L, -1L, 0L)]
        [InlineData(10L, 0L, 0L)]
        [InlineData(0L, 0L, 0L)]
        public void IntegerModulusExecutesCorrectly(long val1, long val2, long expected)
        {
            var inst = new List<Instruction>()
            {
                new Instruction(IntegerConstantToStack, val1),
                new Instruction(IntegerConstantToStack, val2),
                new Instruction(IntegerModulus)
            };
            var actual = ExecuteIntegerFunction(inst);
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
        [InlineData(VmType.Boolean, VmType.Integer, IntegerAdd)]
        [InlineData(VmType.Integer, VmType.Boolean, IntegerAdd)]
        [InlineData(VmType.Boolean, VmType.Boolean, IntegerAdd)]
        [InlineData(VmType.Boolean, VmType.Integer, IntegerSubtract)]
        [InlineData(VmType.Integer, VmType.Boolean, IntegerSubtract)]
        [InlineData(VmType.Boolean, VmType.Boolean, IntegerSubtract)]
        [InlineData(VmType.Boolean, VmType.Integer, IntegerMultiply)]
        [InlineData(VmType.Integer, VmType.Boolean, IntegerMultiply)]
        [InlineData(VmType.Boolean, VmType.Boolean, IntegerMultiply)]
        [InlineData(VmType.Boolean, VmType.Integer, IntegerDivide)]
        [InlineData(VmType.Integer, VmType.Boolean, IntegerDivide)]
        [InlineData(VmType.Boolean, VmType.Boolean, IntegerDivide)]
        [InlineData(VmType.Boolean, VmType.Integer, IntegerModulus)]
        [InlineData(VmType.Integer, VmType.Boolean, IntegerModulus)]
        [InlineData(VmType.Boolean, VmType.Boolean, IntegerModulus)]
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
        [InlineData(0, IntegerAdd)]
        [InlineData(1, IntegerAdd)]
        [InlineData(0, IntegerSubtract)]
        [InlineData(1, IntegerSubtract)]
        [InlineData(0, IntegerMultiply)]
        [InlineData(1, IntegerMultiply)]
        [InlineData(0, IntegerDivide)]
        [InlineData(1, IntegerDivide)]
        [InlineData(0, IntegerModulus)]
        [InlineData(1, IntegerModulus)]
        public void IntegerInstructionRequiringMoreElementsThanOnStackThrows(int numberOfValuesOnStack, InstructionCode instructionCode)
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
