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

    public Block(string body)
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
        parser = new(content);
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
        return content.Replace('\n', '\0');
    }
}