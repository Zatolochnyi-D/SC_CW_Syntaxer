using Syntaxer.Context;
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
    private FileContext context;
    private (int begin, int end) dimension;


    public ScriptFile ParentFile => this;
    public GenericContext Context => context;

    public ScriptFile(string fileContent)
    {
        body = fileContent;
        dimension = (0, body.Length - 1);
        context = new(MemberType.File);
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
        members = blockParser.ParseBody();
        foreach (var member in members)
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