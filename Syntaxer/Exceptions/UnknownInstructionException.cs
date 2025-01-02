namespace Syntaxer.Exceptions;

public class UnknownInstructionException : SyntaxException
{
    private const string INSTRUCTION_NOT_RECOGNIZED_MESSAGE = "Instruction was not recognized.";

    public UnknownInstructionException(int index) : base(index, INSTRUCTION_NOT_RECOGNIZED_MESSAGE) { }
}