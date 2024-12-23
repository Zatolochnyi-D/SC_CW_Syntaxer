namespace Syntaxer;

public class Block : IMember
{
    private string body;
    private string content;
    private List<IMember> members = [];

    public Block(string body)
    {
        this.body = body;
        int indexOfOpenBracket = body.IndexOf('{');
        content = body[indexOfOpenBracket..];
        if (content[^1] == '}')
        {
            content = content[..^1];
        }
    }

    public void SplitContent()
    {
        // members = ScanUtils.ParseBody(content);
    }

    public override string ToString()
    {
        return content.Replace('\n', '\0');
    }
}