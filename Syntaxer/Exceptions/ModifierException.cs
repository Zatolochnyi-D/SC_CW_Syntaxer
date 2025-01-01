namespace Syntaxer.Exceptions;

public class ModifierException : SyntaxException
{
    private const string UNRECOGNIZED_MODIFIER_MESSAGE = "Keyword \"{0}\" is not recognized.";
    private const string DUPLICATE_MODIFIER_MESSAGE = "Keyword \"{0}\" has duplicate(s).";
    private const string CONFLICTED_MODIFIERS_MESSAGE = "Keywords {0} cannot be used together.";

    public ModifierException(int index, string message) : base(index, message) { }

    public static string GetUnrecognizedWordMessage(string word) => string.Format(UNRECOGNIZED_MODIFIER_MESSAGE, word);
    public static string GetDuplicateWordMessage(string word) => string.Format(DUPLICATE_MODIFIER_MESSAGE, word);
    public static string GetConflictedMessage(List<string> words) => string.Format(CONFLICTED_MODIFIERS_MESSAGE, $"\"{string.Join("\", \"", words)}\"");
}