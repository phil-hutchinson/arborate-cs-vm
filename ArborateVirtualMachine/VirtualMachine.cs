using ArborateVirtualMachine.Entity;
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

            foreach(var instruction in Definition.Code)
            {
                CheckInstruction(instruction);
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
    }
}
