namespace ArborateVirtualMachine.Exception
{
    public enum InvalidSourceDetail
    {
        None = 0,
        IncorrectReturnArgumentCount,
        IncorrectReturnArgumentType,
        FunctionDefinitionMissingReturnValue,
        InvalidInstruction,
    }
}