namespace Syntaxer.Exceptions;

public class OpenStringException : SyntaxException
{
    public OpenStringException(int index, string message) : base(index, message) { }
}