namespace Syntaxer.Exceptions;

public class OperatorMisusageException : SyntaxException
{
    private const string OPERATOR_IN_INPROPER_PLACE_MESSAGE = "Operator \"{0}\" is not allowed here.";

    public OperatorMisusageException(int index, string message) : base(index, message) { }

    public static string GetMisplacedOperatorMessage(string operatorString)
    {
        return string.Format(OPERATOR_IN_INPROPER_PLACE_MESSAGE, operatorString);
    }
}