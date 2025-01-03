namespace Syntaxer.Exceptions;

public class MethodDeclarationException : SyntaxException
{   
    public const string DESTRUCTOR_KEYWORD_FOUND = "Keywords detected in destructor declaration, what is not allowed.";
    public const string NO_NAME_FOUND = "Method/constructor/destructor name was not found.";
    public const string TO_MANY_WORDS = "Name containts to many words.";
    public const string DESTRUCTOR_PARAMETERS_FOUND = "Destructor have declared parameters, what is not allowed.";
    public const string MODIFIER_ORDER = "Modifiers must precede name declaration.";
    public const string NOT_ENOUGH_WORDS = "Type or name of method is missing.";

    public MethodDeclarationException(int index, string message) : base(index, message) { }
}