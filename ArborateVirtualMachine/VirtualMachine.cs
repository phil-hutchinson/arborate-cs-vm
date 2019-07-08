using ArborateVirtualMachine.Entity;
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

                }

                instructionNumber = nextInstructionNumber;
            }

            return stack.Pop();
        }
    }
}
