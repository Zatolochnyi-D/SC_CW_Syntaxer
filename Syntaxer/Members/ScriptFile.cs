using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Stores content of individual file.
/// </summary>
public class ScriptFile : IMember
{
    private string body;
    private BlockParser parser;
    private List<int> lineLengths = [];
    private List<IMember> members = [];
    private (int begin, int end) dimension;

    public ScriptFile ParentFile => this;

    public ScriptFile(string fileContent)
    {
        body = fileContent;
        dimension = (0, body.Length - 1); // First and last characters of the file.
        parser = new(body, dimension, this);
        SplitFileIntoLines();
        SplitContent();
        Console.WriteLine(this);
    }

    private void SplitFileIntoLines()
    {
        lineLengths = body.Split('\n').Select(x => x.Length + 1).ToList();
    }

    public void SplitContent()
    {
        members = parser.ParseBody();
        foreach (var member in members) // Testing comment
        {
            member.SplitContent();
        }
    }

    public (int line, int column) IndexToCoordinates(int index)
    {
        int line = 0;
        int column = index;
        for (int i = 0; i < lineLengths.Count; i++)
        {
            column -= lineLengths[i];
            if (column < 0)
            {
                column += lineLengths[i];
                line = i;
                break;
            }
        } // Testing comment ommition.

        return (line, column);
    }

    public override string ToString()
    {
        Console.WriteLine(body.Length);
        string result = "";
        foreach (var member in members)
        {
            result += '\n';
            result += member.ToString();
        }
        return result;
    }
}