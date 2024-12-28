namespace Syntaxer.Exceptions;

public class EmptyEscapeSequenceException : SyntaxException
{
    private const string EMPTY_ESCAPE_SEQUENCE_MESSAGE = "Empty escape sequence was found.";

    public EmptyEscapeSequenceException(int index) : base(index, EMPTY_ESCAPE_SEQUENCE_MESSAGE) { }
}