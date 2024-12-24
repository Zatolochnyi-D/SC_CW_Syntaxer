namespace Syntaxer.Members;

/// <summary>
/// Used for everything that left at the end of block parsing.
/// </summary>
public class Leftover : IMember
{
    private string body;
    private int beginPosition;
    private int endPosition;

    public Leftover(string body, int beginPosition, int endPosition)
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