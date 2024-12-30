namespace Syntaxer.Exceptions;

public class KeywordMisusageException : SyntaxException
{
    private const string FOUND_KEYWORD_DUPLICATION_MESSAGE_WITH_FORMAT = "Duplicate keyword \"{0}\" was found. Only one keyword is allowed.";
    
    public KeywordMisusageException(int index, string message) : base(index, message) { }

    public static string GetFoundDuplicateMessage(string duplicateKeywordName)
    {
        return string.Format(FOUND_KEYWORD_DUPLICATION_MESSAGE_WITH_FORMAT, duplicateKeywordName);
    }
}