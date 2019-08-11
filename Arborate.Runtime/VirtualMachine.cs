using Arborate.Runtime.Entity;
using Arborate.Runtime.Exception;
using static Arborate.Runtime.Entity.InstructionCode;
using static Arborate.Runtime.Exception.InvalidSourceDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using Arborate.Runtime.Utility;

namespace Arborate.Runtime
{
    public class VirtualMachine
    {
        public IList<FunctionDefinition> Definitions { get; }

        public VirtualMachine(params FunctionDefinition[] definitions)
        {
            Definitions = definitions;

            for (int funcIndex = 0; funcIndex < definitions.Length; funcIndex++)
            {
                FunctionDefinition currDef = definitions[funcIndex];
                for (int i = 0; i < currDef.Code.Count; i++)
                {
                    var instruction = currDef.Code[i];
                    CheckInstruction(instruction);
                    CheckInstructionBranch(instruction, i, currDef.Code.Count);
                    CheckInstructionVariable(instruction, currDef.VarCount);
                    CheckInstructionCallFunction(instruction, definitions.Count());
                }

                if (currDef.OutParams.Count == 0)
                {
                    throw new InvalidSourceException(FunctionDefinitionMissingReturnValue);
                }
            }
        }

        private void CheckInstruction(Instruction instruction)
        {
            if (!Enum.IsDefined(typeof(InstructionCode), instruction.InstructionCode)) { 
                throw new InvalidSourceException(InvalidInstruction);
            }

            switch(instruction.InstructionCode)
            {
                case BooleanConstantToStack:
                    if (instruction.Data == null)
                    {
                        throw new InvalidSourceException(MissingInstructionData);
                    }
                    if (!(instruction.Data is bool))
                    {
                        throw new InvalidSourceException(InvalidInstructionData);
                    }
                    break;

                case CallFunction:
                case StackToVariable:
                case VariableToStack:
                case IntegerConstantToStack:
                case Branch:
                case BranchTrue:
                case BranchFalse:
                    if (instruction.Data == null)
                    {
                        throw new InvalidSourceException(MissingInstructionData);
                    }
                    if (!(instruction.Data is long))
                    {
                        throw new InvalidSourceException(InvalidInstructionData);
                    }
                    break;

                default:
                    if (instruction.Data != null)
                    {
                        throw new InvalidSourceException(InstructionCodeDoesNotUseData);
                    }
                    break;
            }
        }

        private void CheckInstructionBranch(Instruction instruction, int instructionPosition, int totalInstructions)
        {
            switch (instruction.InstructionCode)
            {
                case Branch:
                case BranchTrue:
                case BranchFalse:
                    long branchTo = (long)instruction.Data;
                    if (branchTo < 0 || branchTo == instructionPosition || branchTo >= totalInstructions)
                    {
                        throw new InvalidSourceException(InvalidBranchDestination);
                    }
                    break;
            }
        }

        private void CheckInstructionCallFunction(Instruction instruction, int totalFunctions)
        {
            switch (instruction.InstructionCode)
            {
                case CallFunction:
                    long functionCallIndex = (long)instruction.Data;
                    if (functionCallIndex < 0 || functionCallIndex >= totalFunctions)
                    {
                        throw new InvalidSourceException(InvalidFunctionIndex);
                    }
                    break;
            }
        }


        private void CheckInstructionVariable(Instruction instruction, int varCount)
        {
            switch (instruction.InstructionCode)
            {
                case StackToVariable:
                case VariableToStack:
                    long variableIndex = (long)instruction.Data;
                    if (variableIndex < 0 || variableIndex >= varCount)
                    {
                        throw new InvalidSourceException(InvalidVariableIndex);
                    }
                    break;
            }
        }

        public IList<VmValue> Execute(int functionToExecute = 0)
        {
            var stack = new List<VmValue>();
            RunFunction(Definitions[functionToExecute], stack);
            return stack;
        }

        private void RunFunction(FunctionDefinition definition, List<VmValue> stack)
        {
            int previousStackSize = stack.Count - definition.InParams.Count;

            VmBoolean PopBoolean()
            {
                var poppedVal = PopValue();
                if (!(poppedVal is VmBoolean))
                {
                    throw new InvalidSourceException(IncorrectElementTypeOnStack);
                }
                return (VmBoolean)poppedVal;
            }

            VmInteger PopInteger()
            {
                var poppedVal = PopValue();
                if (!(poppedVal is VmInteger))
                {
                    throw new InvalidSourceException(IncorrectElementTypeOnStack);
                }
                return (VmInteger)poppedVal;
            }

            VmValue PopValue()
            {
                if (stack.Count <= previousStackSize)
                {
                    throw new InvalidSourceException(TooFewElementsOnStack);
                }

                var poppedVal = stack.Pop();
                return (VmValue)poppedVal;
            }

            if (stack.Count < definition.InParams.Count)
            {
                throw new InvalidSourceException(TooFewElementsOnStack);
            }

            for (int i = 0; i < definition.InParams.Count; i++)
            {
                if (stack[stack.Count - definition.InParams.Count + i].VmType != definition.InParams[i])
                {
                    throw new InvalidSourceException(IncorrectCallArgumentType);
                }
            }


            var localVariables = Enumerable.Repeat((VmValue)null, definition.VarCount).ToList();

            var instructionNumber = 0;

            while (instructionNumber < definition.Code.Count)
            {
                int nextInstructionNumber = instructionNumber + 1;
                var currentInstruction = definition.Code[instructionNumber];

                switch(currentInstruction.InstructionCode)
                {
                    case CallFunction:
                        {
                            long data = (long)currentInstruction.Data;
                            var defToCall = Definitions[(int)data];

                            RunFunction(defToCall, stack);
                        }
                        break;

                    case StackToVariable:
                        {
                            long data = (long)currentInstruction.Data;
                            var val = PopValue();
                            localVariables[(int)data] = val;
                        }
                        break;

                    case VariableToStack:
                        {
                            long data = (long)currentInstruction.Data;
                            stack.Push(localVariables[(int)data]);
                        }
                        break;

                    case Branch:
                        {
                            long branchTo = (long)currentInstruction.Data;
                            nextInstructionNumber = (int)branchTo;
                        }
                        break;

                    case BranchTrue:
                        {
                            long branchTo = (long)currentInstruction.Data;
                            bool checkVal = PopBoolean().Val;
                            if (checkVal)
                            {
                                nextInstructionNumber = (int)branchTo;
                            }
                        }
                        break;

                    case BranchFalse:
                        {
                            long branchTo = (long)currentInstruction.Data;
                            bool checkVal = PopBoolean().Val;
                            if (!checkVal)
                            {
                                nextInstructionNumber = (int)branchTo;
                            }
                        }
                        break;

                    case BooleanConstantToStack:
                        {
                            bool data = (bool)currentInstruction.Data;
                            var value = new VmBoolean(data);
                            stack.Push(value);
                        }
                        break;

                    case BooleanEqual:
                        {
                            bool val2 = PopBoolean().Val;
                            bool val1 = PopBoolean().Val;
                            var result = val1 == val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case BooleanNotEqual:
                        {
                            bool val2 = PopBoolean().Val;
                            bool val1 = PopBoolean().Val;
                            var result = val1 != val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case BooleanAnd:
                        {
                            bool val2 = PopBoolean().Val;
                            bool val1 = PopBoolean().Val;
                            var result = val1 && val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case BooleanOr:
                        {
                            bool val2 = PopBoolean().Val;
                            bool val1 = PopBoolean().Val;
                            var result = val1 || val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case BooleanNot:
                        {
                            bool val = PopBoolean().Val;
                            var result = !val;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case IntegerConstantToStack:
                        {
                            long data = (long)currentInstruction.Data;
                            var value = new VmInteger(data);
                            stack.Push(value);
                        }
                        break;

                    case IntegerEqual:
                        {
                            long val2 = PopInteger().Val;
                            long val1 = PopInteger().Val;
                            var result = val1 == val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case IntegerNotEqual:
                        {
                            long val2 = PopInteger().Val;
                            long val1 = PopInteger().Val;
                            var result = val1 != val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case IntegerAdd:
                        {
                            long val2 = PopInteger().Val;
                            long val1 = PopInteger().Val;
                            var result = val1 +  val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;

                    case IntegerSubtract:
                        {
                            long val2 = PopInteger().Val;
                            long val1 = PopInteger().Val;
                            var result = val1 - val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;

                    case IntegerMultiply:
                        {
                            long val2 = PopInteger().Val;
                            long val1 = PopInteger().Val;
                            var result = val1 * val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;

                    case IntegerDivide:
                        {
                            long val2 = PopInteger().Val;
                            long val1 = PopInteger().Val;
                            var result = (val2 == 0L) ? 0L : val1 / val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;

                    case IntegerModulus:
                        {
                            long val2 = PopInteger().Val;
                            long val1 = PopInteger().Val;
                            var result = (val2 == 0L) ? 0L : val1 % val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;
                }

                instructionNumber = nextInstructionNumber;
            }

            if (stack.Count != previousStackSize + definition.OutParams.Count)
            {
                throw new InvalidSourceException(IncorrectReturnArgumentCount);
            }

            for (int i = 0; i < definition.OutParams.Count; i++)
            {
                if (stack[stack.Count - definition.OutParams.Count + i].VmType != definition.OutParams[i])
                {
                    throw new InvalidSourceException(IncorrectReturnArgumentType);
                }
            }
        }
    }
}
