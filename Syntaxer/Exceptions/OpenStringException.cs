namespace Syntaxer.Exceptions;

public class OpenStringException : SyntaxException
{
    private const string OPEN_STRING_MESSAGE = "String ending was not found. Ensure second '/\" is present on the same line and is not hidden with escape character \\.";

    public OpenStringException(int index) : base(index, OPEN_STRING_MESSAGE) { }
}