using System;
using System.Collections.Generic;
using System.Text;

namespace ArborateVirtualMachine.Entity
{
    public class Instruction
    {
        public InstructionCode InstructionCode { get; }
        public object Data { get; }

        public Instruction(InstructionCode instructionCode)
        {
            InstructionCode = instructionCode;
            Data = null;
        }

        public Instruction(InstructionCode instructionCode, long data)
        {
            InstructionCode = instructionCode;
            Data = data;
        }

        public Instruction(InstructionCode instructionCode, bool data)
        {
            InstructionCode = instructionCode;
            Data = data;
        }
    }
}
