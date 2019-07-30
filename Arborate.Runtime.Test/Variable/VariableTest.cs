using Arborate.Runtime.Entity;
using Arborate.Runtime.Exception;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Arborate.Runtime.Entity.InstructionCode;
namespace Arborate.Runtime.Test.Variable
{
    public class VariableTest : BaseTest
    {
        #region InstructionCodes
        [Theory]
        [InlineData(1L)]
        [InlineData(2L)]
        [InlineData(3L)]
        public void StackToVariableExecutesCorrectly(long repetitions)
        {
            var constInstructions = new List<Instruction>();
            var varInstuctions = new List<Instruction>();
            for (long i = repetitions; i > 0L; i--)
            {
                constInstructions.Add(new Instruction(IntegerConstantToStack, i));
                varInstuctions.Add(new Instruction(StackToVariable, 0L));
            }

            var instructions = new List<Instruction>();
            instructions.AddRange(constInstructions);
            instructions.Add(new Instruction(IntegerConstantToStack, 1000L));
            instructions.AddRange(varInstuctions);

            var actual = ExecuteIntegerFunction(instructions, varCount: 1);
            Assert.Equal(repetitions, actual);
        }
        #endregion

        #region ThrownExceptions
        [Theory]
        [InlineData(0, StackToVariable)]
        public void VariableOperationRequiringMoreElementsThanOnStackThrows(int numberOfValuesOnStack, InstructionCode instructionCode)
        {
            var instructions = new List<Instruction>();

            for (int i = 0; i < numberOfValuesOnStack; i++)
            {
                instructions.Add(new Instruction(BooleanConstantToStack, false));
            }

            instructions.Add(new Instruction(instructionCode, 0L));

            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteBooleanFunction(instructions, varCount: 1));

            Assert.Equal(InvalidSourceDetail.TooFewElementsOnStack, exception.DetailCode);
        }

        [Theory]
        [InlineData(StackToVariable, -1)]
        [InlineData(StackToVariable, 2)]
        public void VariableOperationOutOfRangeThrows(InstructionCode instructionCode, long variableIndex)
        {
            var instructions = new List<Instruction>()
            {
                new Instruction(instructionCode, variableIndex),
            };

            var exception = Assert.Throws<InvalidSourceException>(() => ExecuteIntegerFunction(instructions, varCount: 2));

            Assert.Equal(InvalidSourceDetail.InvalidVariableIndex, exception.DetailCode);
        }
        #endregion
    }
}
