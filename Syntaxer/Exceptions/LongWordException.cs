namespace Syntaxer.Exceptions;

public class LongWordException : SyntaxException
{
    public const string WORD_INCOMPLETE_MESSAGE = "One ore more words are missing in long word. Check on dots placement.";

    public LongWordException(int index, string message) : base(index, message) { }
}