using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Used to store any separate block.
/// </summary>
public class Block : IMember
{
    private ScriptFile parentFile;
    private IMember parentMember;
    private string body; // part after "{" that holds info inside the block.
    private string identifier; // part before "{" that holds info about block.
    private BlockParser parser;
    private List<IMember> members = [];
    private int beginOfIdentifierPosition;
    private int endOfIdentifierPosition;
    private int beginPosition;
    private int endPosition;


    public Block(string body, int beginPosition, int endPosition, ScriptFile parentFile, IMember parentMember)
    {
        int indexOfOpenBracket = body.IndexOf('{');
        identifier = body[..indexOfOpenBracket];
        if (indexOfOpenBracket != body.Length - 1)
        {
            // Block opening is NOT a last symbol.
            this.body = body[(indexOfOpenBracket + 1)..]; // Take block content excluding "{".
            if (this.body[^1] == '}')
            {
                // Last symbol is closing bracket. Exclude "}".
                this.body = this.body[..^1];
            }
            else
            {
                // Last symbol is not a closing bracket. It should be an error.
            }
        }
        else
        {
            this.body = "";
        }

        for (int i = 0; i < identifier.Length; i++)
        {
            if (!char.IsWhiteSpace(identifier[i]))
            {
                // First non-empty character.
                beginOfIdentifierPosition = beginPosition + i;
            }
        }
        for (int i = 0; i < identifier.Length; i++)
        {
            if (!char.IsWhiteSpace(identifier[^(i + 1)]))
            {
                // Last non-empty character.
                endOfIdentifierPosition = endPosition - i;
            }
        }

        this.beginPosition = beginPosition + indexOfOpenBracket + 1;
        this.endPosition = endPosition - 1;

        parser = new(this.body, beginPosition, endPosition, parentFile, this);
        this.parentFile = parentFile;
        this.parentMember = parentMember;
    }

    public void SplitContent()
    {
        members = parser.ParseBody();
        foreach (var member in members)
        {
            member.SplitContent();
        }
    }

    public override string ToString()
    {
        string result = "";
        (int line, int column) start = parentFile.IndexToCoordinates(beginOfIdentifierPosition);
        (int line, int column) end = parentFile.IndexToCoordinates(endOfIdentifierPosition);
        result += $"s: {beginPosition}, e: {endPosition}; l: {start.line}, c: {start.column}; l: {end.line}, c: {end.column} -> ";
        result += identifier.Replace('\n', '\0').Trim();
        foreach (var member in members)
        {
            result += '\n';
            result += member.ToString();
        }
        return result;
    }
}