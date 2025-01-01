namespace Syntaxer.Exceptions;

public class ClassDeclarationException : SyntaxException
{
    public const string NO_NAME_DECLARED_MESSAGE = "No name of class was found. Give one.";
    public const string INCORRECT_PLACE_MESSAGE = "Class was found in incorrect place. Class can be placed only in file, namespace, interface or other class.";
    public const string MANY_NAMES_DECLARED_MESSAGE = "Several names for the class were found. Select only one.";

    public ClassDeclarationException(int index, string message) : base(index, message) { }
}