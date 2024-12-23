using Syntaxer.Parsers;

namespace Syntaxer;

public class ScriptFile : IMember
{
    private string body;
    private BlockParser parser;
    private List<string> linesOfFile = [];
    private List<IMember> members = [];

    public ScriptFile(string body)
    {
        this.body = body;
        parser = new(body);
        SplitFileIntoLines();
        SplitContent();
        foreach (var m in members)
        {
            Console.WriteLine(m);
        }
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
        members = parser.ParseBody();
    }
}