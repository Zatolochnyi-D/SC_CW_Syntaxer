namespace Syntaxer.Exceptions;

public class DelegateException : SyntaxException
{
    public const string BRACKETS_ERROR = "Parameter brackets weren't specified correctly.";
    public const string NO_NAME_PROVIDED = "Delegate name was not provided.";
    public const string MANY_NAMES_PROVIDED = "Several names were found in delegate.";
    public const string TYPE_NOT_FOUND = "Delegate type not found.";

    public DelegateException(int index, string message) : base(index, message) { }
}