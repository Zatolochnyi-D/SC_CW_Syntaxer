namespace Syntax;

public class Instruction
{
    private string originalBody;

    public Instruction(string body)
    {
        originalBody = body;
    }
    public override string ToString()
    {
        return originalBody.Replace('\n', '\0');
    }
}