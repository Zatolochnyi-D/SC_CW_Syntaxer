namespace Syntaxer.Exceptions;

public class EnumDeclarationException : SyntaxException
{
    private const string KEYWORD_AS_NAME_MESSAGE = "Keyword \"{0}\" used as name, what is not right.";
    public const string NO_NAME_DECLARED_MESSAGE = "No name of enum was found. Give one.";
    public const string INCORRECT_PLACE_MESSAGE = "Enum was found in incorrect place. Enum can be placed only in file, namespace, interface or other class.";
    public const string MANY_NAMES_DECLARED_MESSAGE = "Several names for the enum were found. Select only one.";
    public const string DECLARATION_MISSING_MESSAGE = "Enum body have missing \",\".";

    public EnumDeclarationException(int index, string message) : base(index, message) { }

    public static string GetKeywordAsNameMessage(string keyword) => string.Format(KEYWORD_AS_NAME_MESSAGE, keyword);
}