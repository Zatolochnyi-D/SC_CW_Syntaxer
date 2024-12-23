namespace Syntaxer;

public class ScriptFile : IMember
{
    private string originalBody;
    private List<string> linesOfFile = [];
    private List<object> members = [];

    public ScriptFile(string body)
    {
        originalBody = body;
        SplitFileIntoLines();
        SplitContent();
    }

    private void SplitFileIntoLines()
    {
        string line = "";
        foreach (var symbol in originalBody)
        {
            if (symbol == '\n')
            {
                linesOfFile.Add(line);
                line = "";
                continue;
            }
            line += symbol;
        }
        linesOfFile.Add(line);
    }

    public void SplitContent()
    {
        
    }
}