namespace Syntaxer.Exceptions;

public class NamespaceDeclarationException : SyntaxException
{
    public const string KEYWORD_IS_NOT_FIRST_MESSAGE = "\"namespace\" keyword is found but it's not first.";
    public const string INCORRECT_PLACE_MESSAGE = "Namespace was found in incorrect place. Namespace can be declared only in file's top level or in other namespace.";
    public const string MANY_NAMES_DECLARED_MESSAGE = "Several names for the namespace was found. Select only one.";
    public const string ANY_NAME_DECLARED_MESSAGE = "No name of namespace was found. Give one.";

    public NamespaceDeclarationException(int index, string message) : base(index, message) { }
}