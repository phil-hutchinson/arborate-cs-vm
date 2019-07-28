﻿using ArborateVirtualMachine.Entity;
using ArborateVirtualMachine.Exception;
using static ArborateVirtualMachine.Entity.InstructionCode;
using static ArborateVirtualMachine.Exception.InvalidSourceDetail;
using System;
using System.Collections.Generic;

namespace ArborateVirtualMachine
{
    public class VirtualMachine
    {
        public FunctionDefinition Definition { get; }

        public VirtualMachine(FunctionDefinition definition)
        {
            Definition = definition;

            for(int i = 0; i < Definition.Code.Count; i++)
            {
                var instruction = Definition.Code[i];
                CheckInstruction(instruction);
                CheckInstructionBranch(instruction, i, Definition.Code.Count);
            }

            if (definition.OutParams.Count == 0)
            {
                throw new InvalidSourceException(FunctionDefinitionMissingReturnValue);
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

        public VmValue Execute()
        {
            return RunFunction(Definition);
        }

        private VmValue RunFunction(FunctionDefinition definition)
        {
            var stack = new Stack<VmValue>();
            var instructionNumber = 0;

            while (instructionNumber < definition.Code.Count)
            {
                int nextInstructionNumber = instructionNumber + 1;
                var currentInstruction = definition.Code[instructionNumber];

                switch(currentInstruction.InstructionCode)
                {
                    case Branch:
                        {
                            long branchTo = (long)currentInstruction.Data;
                            nextInstructionNumber = (int)branchTo;
                        }
                        break;

                    case BranchTrue:
                        {
                            long branchTo = (long)currentInstruction.Data;
                            bool checkVal = PopBoolean(stack).Val;
                            if (checkVal)
                            {
                                nextInstructionNumber = (int)branchTo;
                            }
                        }
                        break;

                    case BranchFalse:
                        {
                            long branchTo = (long)currentInstruction.Data;
                            bool checkVal = PopBoolean(stack).Val;
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
                            bool val2 = PopBoolean(stack).Val;
                            bool val1 = PopBoolean(stack).Val;
                            var result = val1 == val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case BooleanNotEqual:
                        {
                            bool val2 = PopBoolean(stack).Val;
                            bool val1 = PopBoolean(stack).Val;
                            var result = val1 != val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case BooleanAnd:
                        {
                            bool val2 = PopBoolean(stack).Val;
                            bool val1 = PopBoolean(stack).Val;
                            var result = val1 && val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case BooleanOr:
                        {
                            bool val2 = PopBoolean(stack).Val;
                            bool val1 = PopBoolean(stack).Val;
                            var result = val1 || val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case BooleanNot:
                        {
                            bool val = PopBoolean(stack).Val;
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
                            long val2 = PopInteger(stack).Val;
                            long val1 = PopInteger(stack).Val;
                            var result = val1 == val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case IntegerNotEqual:
                        {
                            long val2 = PopInteger(stack).Val;
                            long val1 = PopInteger(stack).Val;
                            var result = val1 != val2;
                            stack.Push(new VmBoolean(result));
                        }
                        break;

                    case IntegerAdd:
                        {
                            long val2 = PopInteger(stack).Val;
                            long val1 = PopInteger(stack).Val;
                            var result = val1 +  val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;

                    case IntegerSubtract:
                        {
                            long val2 = PopInteger(stack).Val;
                            long val1 = PopInteger(stack).Val;
                            var result = val1 - val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;

                    case IntegerMultiply:
                        {
                            long val2 = PopInteger(stack).Val;
                            long val1 = PopInteger(stack).Val;
                            var result = val1 * val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;

                    case IntegerDivide:
                        {
                            long val2 = PopInteger(stack).Val;
                            long val1 = PopInteger(stack).Val;
                            var result = (val2 == 0L) ? 0L : val1 / val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;

                    case IntegerModulus:
                        {
                            long val2 = PopInteger(stack).Val;
                            long val1 = PopInteger(stack).Val;
                            var result = (val2 == 0L) ? 0L : val1 % val2;
                            stack.Push(new VmInteger(result));
                        }
                        break;
                }

                instructionNumber = nextInstructionNumber;
            }

            if (stack.Count != definition.OutParams.Count)
            {
                throw new InvalidSourceException(IncorrectReturnArgumentCount, $"Incorrect number of elements on stack at function exit (expected {definition?.OutParams?.Count}, actual {stack?.Count}).");
            }

            var stackEnum = stack.GetEnumerator();
            var outParamEnum = definition.OutParams.GetEnumerator();
            while (stackEnum.MoveNext() && outParamEnum.MoveNext())
            {
                if (stackEnum.Current.VmType != outParamEnum.Current)
                {
                    throw new InvalidSourceException(IncorrectReturnArgumentType, $"Incorrect element type on stack at function exit (expected {outParamEnum?.Current.ToString()}, actual {stackEnum.Current.ToString()}).");
                }
            }

            return stack.Pop();
        }

        private VmBoolean PopBoolean(Stack<VmValue> stack)
        {
            if (stack.Count == 0)
            {
                throw new InvalidSourceException(TooFewElementsOnStack);
            }

            var poppedVal = stack.Pop();
            if (!(poppedVal is VmBoolean))
            {
                throw new InvalidSourceException(IncorrectElementTypeOnStack);
            }
            return (VmBoolean)poppedVal;
        }

        private VmInteger PopInteger(Stack<VmValue> stack)
        {
            if (stack.Count == 0)
            {
                throw new InvalidSourceException(TooFewElementsOnStack);
            }

            var poppedVal = stack.Pop();
            if (!(poppedVal is VmInteger))
            {
                throw new InvalidSourceException(IncorrectElementTypeOnStack);
            }
            return (VmInteger)poppedVal;
        }
    }
}