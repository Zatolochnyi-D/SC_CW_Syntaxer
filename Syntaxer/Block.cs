namespace Syntax;

public class Block
{
    private string originalBody;

    public Block(string body)
    {
        originalBody = body;
    }

    public override string ToString()
    {
        return originalBody.Replace('\n', '\0');
    }
}