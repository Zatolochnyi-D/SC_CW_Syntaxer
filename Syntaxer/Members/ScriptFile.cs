using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Stores content of individual file.
/// </summary>
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
        Console.WriteLine(this);
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
        foreach (var member in members)
        {
            member.SplitContent();
        }
    }

    public override string ToString()
    {
        string result = "";
        foreach (var member in members)
        {
            result += member.ToString();
            result += '\n';
        }
        return result;
    }
}