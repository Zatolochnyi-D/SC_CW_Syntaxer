namespace Syntaxer.Exceptions;

public class SyntaxException : Exception
{
    private int index;

    public int Index => index;

    public SyntaxException(int index, string message) : base(message)
    {
        this.index = index;
    }
}