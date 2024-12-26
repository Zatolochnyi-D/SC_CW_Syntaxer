namespace Syntaxer.Exceptions;

public class OpenStringException : ParserException
{
    private int indexOfOpening;

    public int IndexOfOpening => indexOfOpening;

    public OpenStringException(string message, int indexOfOpening) : base(message)
    {
        this.indexOfOpening = indexOfOpening;
    }
}