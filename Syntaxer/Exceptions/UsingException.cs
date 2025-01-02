namespace Syntaxer.Exceptions;

public class UsingException : SyntaxException
{
    public const string KEYWORD_IS_NOT_FIRST_MESSAGE = "\"using\" keyword is found but it's not first.";
    public const string INCORRECT_PLACE_MESSAGE = "Using directive was found in incorrect place. They allowed only on the beginning of the file.";
    public const string NO_NAME_DECLARED_MESSAGE = "No namespace specified for import.";
    public const string MANY_NAMES_DECLARED_MESSAGE = "Several namespaces were declared for import. Select only one.";

    public UsingException(int index, string message) : base(index, message) { }
}