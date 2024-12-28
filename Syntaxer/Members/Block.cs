using Syntaxer.Context;
using Syntaxer.Exceptions;
using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Used to store any separate block.
/// </summary>
public class Block : IMember
{
    private ScriptFile parentFile;
    private IMember parent;
    private string identifier; // part before "{" that holds info about block.
    private string body; // part after "{" that holds info inside the block.
    private (int begin, int end) wholeDimension;
    private (int begin, int end) identifierDimension;
    private (int begin, int end) bodyDimension;
    private BlockParser parser;
    private InstructionParser identifierParser;
    private List<IMember> members = [];
    private GenericContext context;

    public ScriptFile ParentFile => parentFile;
    public GenericContext Context => context;

    public List<SyntaxException> Exceptions => throw new NotImplementedException();

    public Block(string blockContent, (int, int) dimension, IMember parent)
    {
        this.parent = parent;
        parentFile = parent.ParentFile;

        // blockContent always have "{", as it is a block defining symbol. If there are several "{" symbols. First one is what we need.
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
            throw new Exception(); // placeholder
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
            throw new Exception(); // placeholder
        }

        // For the next step we need to properly identify begin and end for both identifier and body.
        // Given dimension begin is the next symbol after previous instruction/block.
        // End is the position of last "}" (considering we excluded cases when "}" is missing before).

        wholeDimension = dimension;

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

        parser = new(body, bodyDimension, this);
        identifierParser = new(identifier, bodyDimension, this);
        context = identifierParser.ParseBody();
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