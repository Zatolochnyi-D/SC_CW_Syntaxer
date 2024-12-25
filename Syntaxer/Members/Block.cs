using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Used to store any separate block.
/// </summary>
public class Block : IMember
{
    private ScriptFile parentFile;
    private IMember parentMember;
    private string body;
    private string identifier; // part before "{" that holds info about block.
    private string content; // part after "{" that holds info inside the block.
    private BlockParser parser;
    private List<IMember> members = [];
    private int beginPosition;
    private int endPosition;


    public Block(string body, int beginPosition, int endPosition, ScriptFile parentFile, IMember parentMember)
    {
        this.body = body;
        int indexOfOpenBracket = body.IndexOf('{');
        identifier = body[..indexOfOpenBracket];
        if (indexOfOpenBracket != body.Length - 1)
        {
            // Block opening is NOT a last symbol.
            content = body[(indexOfOpenBracket + 1)..];
            if (content[^1] == '}') content = content[..^1];
        }
        else
        {
            content = "";
        }
        this.beginPosition = beginPosition;
        this.endPosition = endPosition;
        parser = new(content, beginPosition, endPosition, parentFile, this);
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
        (int line, int column) start = parentFile.IndexToCoordinates(beginPosition);
        (int line, int column) end = parentFile.IndexToCoordinates(endPosition);
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