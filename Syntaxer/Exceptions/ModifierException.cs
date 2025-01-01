namespace Syntaxer.Exceptions;

public class ModifierException : SyntaxException
{
    private const string NON_MODIFIER_MESSAGE = "Word \"{0}\" is used like modifier, when it's not one.";
    private const string WRONG_CONTEXT_MESSAGE = "Keyword \"{0}\" is not allowed in current context.";
    private const string DUPLICATE_MODIFIER_MESSAGE = "Keyword \"{0}\" has duplicate(s).";
    private const string CONFLICTED_MODIFIERS_MESSAGE = "Keywords {0} cannot be used together.";

    public ModifierException(int index, string message) : base(index, message) { }

    public static string GetNonModifierMessage(string word) => string.Format(NON_MODIFIER_MESSAGE, word);
    public static string GetWrongContextMessage(string word) => string.Format(WRONG_CONTEXT_MESSAGE, word);
    public static string GetDuplicateWordMessage(string word) => string.Format(DUPLICATE_MODIFIER_MESSAGE, word);
    public static string GetConflictedMessage(IEnumerable<string> words) => string.Format(CONFLICTED_MODIFIERS_MESSAGE, $"\"{string.Join("\", \"", words)}\"");
}