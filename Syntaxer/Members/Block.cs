using Syntaxer.Context;
using Syntaxer.Exceptions;
using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Stores blocks identifying sequence and contents. Responsible for it's scanning.
/// </summary>
public class Block : IMember
{
    private ScriptFile parentFile;
    private IMember parent;
    private string identifier; // part before "{" that holds info about block.
    private string? body = null; // part after "{" that holds info inside the block. If body is null - open bracket exception happened.
    private List<SyntaxException> exceptions = [];
    private (int begin, int end) wholeDimension;
    private (int begin, int end) identifierDimension;
    private (int begin, int end) bodyDimension;
    private List<IMember> members = [];
    private GenericContext context;

    public ScriptFile ParentFile => parentFile;
    public IMember Parent => parent;
    public List<SyntaxException> Exceptions => throw new NotImplementedException();
    public GenericContext Context => throw new NotImplementedException();

    public Block(string blockContent, (int, int) dimension, IMember parent, int bracketBalance)
    {
        this.parent = parent;
        parentFile = parent.ParentFile;
        wholeDimension = dimension;

        // blockContent always have "{", as it is a block defining symbol.
        // By identifying position of first "{", we can split content to identifier and block body.
        int indexOfOpenBracket = blockContent.IndexOf('{');
        identifier = blockContent[..indexOfOpenBracket];
        if (indexOfOpenBracket != blockContent.Length - 1)
        {
            // Sometimes "{" can be the last symbol, so we must ensure it's not.
            body = blockContent[(indexOfOpenBracket + 1)..]; // Take block content after "{" (excluding "{").
        }
        else 
        {
            // If there are no symbols after "{", throw an error and stop futher scan (there is no guarantee of identifying more errors correctly).
            // "Block is defined, by is not closed."
            exceptions.Add(new OpenBlockException(wholeDimension.begin + indexOfOpenBracket));
            return;
        }

        // Block ends either by "}" or end of the line. If first, removing "}" from body ensures we have clean block body for scanning.
        // If second, throw error and stop scan.
        if (body[^1] == '}')
        {
            // Last symbol is closing bracket. Exclude "}".
            body = body[..^1];
        }
        else
        {
            exceptions.Add(new OpenBlockException(wholeDimension.begin + indexOfOpenBracket));
            return;
        }

        // Check if bracket balance of this block is 0 (balanced). If not - exception.
        if (bracketBalance != 0)
        {
            exceptions.Add(new OpenBlockException(wholeDimension.begin + indexOfOpenBracket));
            return;
        }

        // For the next step we need to properly identify begin and end for both identifier and body.
        // Given dimension begin is the next symbol after previous instruction/block. End is the position of last "}".

        // Begin of identifier is first non-empty character after dimension.begin. End of identifier is first non-empty character before "{"
        identifierDimension = (wholeDimension.begin, wholeDimension.begin + indexOfOpenBracket - 1);
        bool beginFound = false;
        bool endFound = false;
        for (int i = 0; i < identifier.Length; i++)
        {
            if (!beginFound) if (!char.IsWhiteSpace(identifier[i]))
            {
                // First non-empty character.
                identifierDimension.begin += i;
                beginFound = true;
            }
            if (!endFound) if (!char.IsWhiteSpace(identifier[^(i + 1)]))
            {
                // Last non-empty character.
                identifierDimension.end -= i;
                endFound = true;
            }
        }

        // Begin of body is first character after "{". End of body is first character before "}".
        // (Considering we excluded cases when there is nothing after "{" and there is not an "}" in the end).
        bodyDimension = (wholeDimension.begin + indexOfOpenBracket + 1, wholeDimension.end - 1);
    }

    public void SplitContent()
    {
        if (body == null)
        {
            // Open Bracket Exception.
            return;
        }
        // Scan identifier first.
        var identifierParser = new InstructionParser(identifier, identifierDimension, this);
        identifierParser.ParseBody();
        var bodyParser = new BlockParser(body, bodyDimension, this);
        // bodyParser.ParseBody();
        // members = bodyParser.Members;
        // foreach (var member in members)
        // {
        //     member.SplitContent();
        // }
    }

    public List<SyntaxException> CollectExceptions()
    {
        List<SyntaxException> result = [];
        result.AddRange(exceptions);
        result.AddRange(members.SelectMany(x => x.CollectExceptions()));
        return result;
    }

    public override string ToString()
    {
        string result = "";
        (int line, int column) start = parentFile.IndexToCoordinates(identifierDimension.begin);
        (int line, int column) end = parentFile.IndexToCoordinates(identifierDimension.end);
        result += $"s: {identifierDimension.begin}, e: {identifierDimension.end}; l: {start.line}, c: {start.column}; l: {end.line}, c: {end.column} -> ";
        result += identifier.Replace('\n', '\0').Trim();
        foreach (var member in members)
        {
            result += '\n';
            result += member.ToString();
        }
        return result;
    }
}