namespace Syntaxer.Exceptions;

public class ParameterException : SyntaxException
{
    public const string TO_MANY_NAMES = "One of parameters have to many names declared.";
    public const string KEYWORDS_FOUND = "Keywords were found in parameters.";
    public const string REF_OUT_NOT_FIRST = "\"ref\" or \"out\" are not first.";
    public const string MISSING_PARAMETER = "\",\" was found but no parameter after.";
    public const string OPERATOR_FOUND = "Operator was found what is not allowed";

    public ParameterException(int index, string message) : base(index, message) { }
}