using Syntaxer.Enumerations;
using Syntaxer.Exceptions;
using Syntaxer.Literals;
using Syntaxer.Members;
using Syntaxer.Parsers;

namespace Syntaxer.Validators;

/// <summary>
/// Class responsible for identifying erroneous strings if file.
/// Collects correct strings and errors for incorrect strings.
/// </summary>
public class StringValidator : Parser
{
    private ScriptFile parent;
    private List<SyntaxException> foundExceptions = [];
    private List<StringLiteral> foundCorrectStrings = [];

    public List<SyntaxException> FoundExceptions => foundExceptions;
    public List<StringLiteral> FoundCorrectStrings => foundCorrectStrings;

    public StringValidator(string fileContent, ScriptFile parent) : base(fileContent)
    {
        this.parent = parent;
    }

    private int SkipString(int position, StringType type)
    {
        int start = position;
        char terminationSymbol;
        switch (type)
        {
            case StringType.String:
                terminationSymbol = '"';
                break;
            case StringType.Char:
                terminationSymbol = '\'';
                break;
            default:
                throw new NotImplementedException($"Unknown type was given - {type}");
        }

        string stringBody = "";
        stringBody += Body[position];
        while (true)
        {
            // Move position forward and write contents. Look for termination symbol.
            position++;
            if (IsEndOfBody(position) || Body[position] == '\n')
            {
                // That means the string was not closed. Futher scan is impossible.
                foundExceptions.Add(new OpenStringException(start));
                break;
            }
            else if (Body[position] == '\\')
            {
                foundExceptions.Add(new EmptyEscapeSequenceException(start));
            }
            stringBody += Body[position];
            if (Body[position] == terminationSymbol)
            if (IsStringTerminator(position))
            {
                // If string found it's end successfully (otherwise this part will never run), create string literal.
                foundCorrectStrings.Add(new(stringBody, (start, position)));
                break;
            }
        }
        return position;
    }

    public void Validate()
    {
        for (int i = 0; i < Body.Length; i++)
        {
            // Handle comments
            if (Body[i] == '/')
            {
                if (IsLastSymbol(i))
                {
                    // Last symbol. There can't be comments.
                }
                else if (Body[i + 1] == '/')
                {
                    // It's a comment.
                    i = SilentSkipComment(i);
                }
                continue;
            }
            else if (Body[i] == '\'')
            {
                // Char declarator found.
                i = SkipString(i, StringType.Char);
                continue;
            }
            else if (Body[i] == '"')
            {
                // String declarator found.
                i = SkipString(i, StringType.String);
                continue;
            }
        }
    }
}