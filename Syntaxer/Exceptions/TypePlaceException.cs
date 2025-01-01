namespace Syntaxer.Exceptions;

public class TypePlaceException : SyntaxException
{
    private const string MESSAGE = "Type declared in wrong place.";

    public TypePlaceException(int index) : base(index, MESSAGE) { }
}