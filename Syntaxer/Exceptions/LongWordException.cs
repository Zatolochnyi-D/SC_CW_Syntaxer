namespace Syntaxer.Exceptions;

public class LongWordException : SyntaxException
{
    private const string KEYWORD_USED_MESSAGE = "Keyword \"{0}\" was found in name.";
    private const string INVALID_SYMBOLS_MESSAGE = "Name \"{0}\" does not contain any valid symbols.";
    public const string WORD_INCOMPLETE_MESSAGE = "One ore more words are missing in long word. Check on dots placement.";

    public LongWordException(int index, string message) : base(index, message) { }

    public static string GetKeywordFoundMessage(string keyword) => string.Format(KEYWORD_USED_MESSAGE, keyword);
    public static string GetInvalidSymbolsMessage(string symbols) => string.Format(INVALID_SYMBOLS_MESSAGE, symbols);
}