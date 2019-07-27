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
        #endregion
    }
}
