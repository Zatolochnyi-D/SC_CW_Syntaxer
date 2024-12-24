using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Used to store any separate block.
/// </summary>
public class Block : IMember
{
    private string body;
    private string identifier; // part before "{" that holds info about block.
    private string content; // part after "{" that holds info inside the block.
    private BlockParser parser;
    private List<IMember> members = [];
    private int beginPosition;
    private int endPosition;


    public Block(string body, int beginPosition, int endPosition)
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
        parser = new(content, beginPosition, endPosition);
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
        result += $"{beginPosition}, {endPosition}: ";
        result += identifier.Replace('\n', '\0').Trim();
        foreach (var member in members)
        {
            result += '\n';
            result += member.ToString();
        }
        return result;
    }
}