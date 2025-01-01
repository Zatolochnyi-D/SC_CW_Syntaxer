namespace Syntaxer.Exceptions;

public class UnknownBlockException : SyntaxException
{
    private const string BLOCK_NOT_RECOGNIZED_MESSAGE = "Block was not recognized.";

    public UnknownBlockException(int index) : base(index, BLOCK_NOT_RECOGNIZED_MESSAGE) { }
}