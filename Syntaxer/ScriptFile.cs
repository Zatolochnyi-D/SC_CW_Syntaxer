namespace Syntaxer;

public class ScriptFile : IMember
{
    private string body;
    private List<string> linesOfFile = [];
    private List<IMember> members = [];

    public ScriptFile(string body)
    {
        this.body = body;
        SplitFileIntoLines();
        SplitContent();
    }

    private void SplitFileIntoLines()
    {
        string line = "";
        foreach (var symbol in body)
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
        members = ScanUtils.ParseBody(body);
    }
}