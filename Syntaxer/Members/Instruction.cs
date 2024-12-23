namespace Syntaxer;

public class Instruction : IMember
{
    private string body;

    public Instruction(string body)
    {
        this.body = body;
    }

    public void SplitContent()
    {
        
    }

    public override string ToString()
    {
        return body.Replace('\n', '\0');
    }
}