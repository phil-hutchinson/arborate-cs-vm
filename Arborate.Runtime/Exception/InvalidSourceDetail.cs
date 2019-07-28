namespace Arborate.Runtime.Exception
{
    public enum InvalidSourceDetail
    {
        None = 0,
        IncorrectReturnArgumentCount,
        IncorrectReturnArgumentType,
        FunctionDefinitionMissingReturnValue,
        InvalidInstruction,
        MissingInstructionData,
        InvalidInstructionData,
        InstructionCodeDoesNotUseData,
        IncorrectElementTypeOnStack,
        TooFewElementsOnStack,
        InvalidBranchDestination,
    }
}