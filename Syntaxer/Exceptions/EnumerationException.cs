namespace Syntaxer.Exceptions;

public class EnumeratioException : SyntaxException
{
    public const string OPERATION_ENUMERATION_INCOMPLETE = "Some parts are missing.";

    public EnumeratioException(int index) : base(index, OPERATION_ENUMERATION_INCOMPLETE) { }
}