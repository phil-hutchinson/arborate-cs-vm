using System;
using System.Collections.Generic;
using System.Text;

namespace ArborateVirtualMachine.Entity
{
    public enum InstructionCode
    {
        // stack operations
        VariableToStack = 0x0010,
        StackToVariable = 0x0011,

        // function operations
        CallFunction = 0x0020,
        ExitFunction = 0x0021,

        // branch
        Branch = 0x0030,
        BranchTrue = 0x0031,
        BranchFalse = 0x0032,

        // boolean operations
        BooleanConstantToStack = 0x0100,
        BooleanEqual = 0x0101,
        BooleanNotEqual = 0x0102,
        BooleanAnd = 0x0103,
        BooleanOr = 0x0107,
        BooleanNot = 0x0108,

        // integer operations
        IntegerConstantToStack = 0x0500,
        IntegerEqual = 0x0501,
        IntegerNotEqual = 0x0502,
        IntegerAdd = 0x0503,
        IntegerSubtract = 0x0504,
        IntegerMultiply = 0x0505,
        IntegerDivide = 0x0506,
        IntegerModulus = 0x0507,
        IntegerShiftLeft = 0x0508,
        IntegerShiftRightArithmetic = 0x0509,
        IntegerShiftRightLogical = 0x050A,
        IntegerRotateLeft = 0x050B,
        IntegerRotateRight = 0x050C,
        IntegerBitwiseAnd = 0x050D,
        IntegerBitwise_or = 0x050E,
        IntegerBitwiseNot = 0x050F,
        IntegerBitwiseXor = 0x0510,
        IntegerGreaterThan = 0x0511,
        IntegerLessThan = 0x0512,
        IntegerGreaterEqual = 0x0513,
        IntegerLessEqual = 0x0514,
    }
}
