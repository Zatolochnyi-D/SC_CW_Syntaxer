namespace Syntaxer.Members;

/// <summary>
/// Used for everything that left at the end of block parsing.
/// </summary>
public class Leftover : IMember
{
    private string body;

    public Leftover(string body)
    {
        this.body = body;
    }

    public void SplitContent()
    {
        
    }
}