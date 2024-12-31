using Syntaxer.Exceptions;

namespace Syntaxer.Validators;

public class NameValidator
{
    private List<string> words = [];
    private int position;
    private List<SyntaxException> exceptions = [];

    public List<SyntaxException> Exceptions => exceptions;

    public NameValidator(int position, params string[] words)
    {
        this.position = position;
        this.words.AddRange(words);
    }

    public void Validate()
    {
        foreach (var word in words)
        {
            if (word == "") continue;
            if (!word.Any(char.IsLetterOrDigit))
            {
                // Word does not have any letters in it.
                exceptions.Add(new LongWordException(position, LongWordException.GetInvalidSymbolsMessage(word)));

            }
            else if (Keywords.ALL_KEYWORDS.Contains(word))
            {
                // The word is a keyword, what is not allowed.
                exceptions.Add(new LongWordException(position, LongWordException.GetKeywordFoundMessage(word)));
            }
        }
    }
}