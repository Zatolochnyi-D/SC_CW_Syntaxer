namespace Syntaxer.Exceptions;

public class OpenBlockException : SyntaxException
{
    private const string OPEN_BLOCK_MESSAGE = "Block ending was not found.";

    public OpenBlockException(int index) : base(index, OPEN_BLOCK_MESSAGE) { }
}