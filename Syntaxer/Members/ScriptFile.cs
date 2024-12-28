using Syntaxer.Exceptions;
using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Class responsible for storing contents of one file. Works with it's members and is a way fo members to communicate with each other.
/// </summary>
public class ScriptFile : IMember
{
    private string body;
    private List<int> lineLengths = [];
    private List<SyntaxException> exceptions = [];
    private List<IMember> members = [];
    private (int begin, int end) dimension;

    public ScriptFile ParentFile => this;
    public List<SyntaxException> Exceptions => exceptions;

    public ScriptFile(string fileContent)
    {
        body = fileContent;
        dimension = (0, body.Length - 1);
        SplitFileIntoLines();
    }

    private void SplitFileIntoLines()
    {
        lineLengths = body.Split('\n').Select(x => x.Length + 1).ToList();
    }

    public void SplitContent()
    {
        var stringParser = new StringParser(body);
        stringParser.ParseBody();
        exceptions.AddRange(stringParser.FoundExceptions);
        if (stringParser.FoundExceptions.Select(x => x is OpenStringException).Count() != 0)
        {
            // There are open string errors, so futher scan should be stopped.
            return;
        }

        var blockParser = new BlockParser(body, dimension, this);
        blockParser.ParseBody();
        members = blockParser.Members;
        foreach (var member in members)
        {
            member.SplitContent();
        }
    }

    public List<SyntaxException> CollectExceptions()
    {
        List<SyntaxException> result = [];
        result.AddRange(exceptions);
        result.AddRange(members.SelectMany(x => x.CollectExceptions()));
        return result;
    }

    public List<string> ExceptionsAsStrings()
    {
        List<string> result = [];
        foreach (var exception in CollectExceptions())
        {
            (int line, int column) coords = IndexToCoordinates(exception.Index);
            result.Add($"At line: {coords.line}, column: {coords.column}:    {exception.Message}");
        }
        return result;
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
        }

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