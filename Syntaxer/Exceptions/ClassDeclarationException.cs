namespace Syntaxer.Exceptions;

public class ClassDeclarationException : SyntaxException
{
    private const string KEYWORD_AS_NAME_MESSAGE = "Keyword \"{0}\" used as name, what is not right.";
    public const string NO_NAME_DECLARED_MESSAGE = "No name of class was found. Give one.";
    public const string INCORRECT_PLACE_MESSAGE = "Class was found in incorrect place. Class can be placed only in file, namespace, interface or other class.";
    public const string MANY_NAMES_DECLARED_MESSAGE = "Several names for the class were found. Select only one.";
    public const string INHERITANCE_MISSING_MESSAGE = "Inheritance is declared but no names provided.";
    public const string COMMA_MISSED_IN_INHERITANCE = "\",\" symbol missed in inheritance part.";

    public ClassDeclarationException(int index, string message) : base(index, message) { }

    public static string GetKeywordAsNameMessage(string keyword) => string.Format(KEYWORD_AS_NAME_MESSAGE, keyword);
}