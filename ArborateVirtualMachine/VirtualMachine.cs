﻿using ArborateVirtualMachine.Entity;
using ArborateVirtualMachine.Exception;
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

            if (definition.OutParams.Count == 0)
            {
                throw new InvalidSourceException(InvalidSourceDetail.FunctionDefinitionMissingReturnValue);
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
                    case InstructionCode.BooleanConstantToStack:
                        {
                            bool data = (bool)currentInstruction.Data;
                            var value = new VmBoolean(data);
                            stack.Push(value);
                        }
                        break;

                    case InstructionCode.IntegerConstantToStack:
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
    }
}
