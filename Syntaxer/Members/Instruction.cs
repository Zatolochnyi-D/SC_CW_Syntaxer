namespace Syntaxer.Members;

/// <summary>
/// Used to store any code that ends with ";"
/// </summary>
public class Instruction : IMember
{
    private string body;
    private int beginPosition;
    private int endPosition;


    public Instruction(string body, int beginPosition, int endPosition)
    {
        this.body = body;
        this.beginPosition = beginPosition;
        this.endPosition = endPosition;
    }

    public void SplitContent()
    {

    }

    public override string ToString()
    {
        string result = "";
        result += $"{beginPosition}, {endPosition}: ";
        result += body.Replace('\n', '\0').Trim();
        return result;
    }
}