namespace Syntaxer.Exceptions;

public class LeftoverException : SyntaxException
{
    private const string MESSAGE = "Unidentified part of code found.";

    public LeftoverException(int index) : base(index, MESSAGE) { }
}