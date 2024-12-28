
using Syntaxer.Context;
using Syntaxer.Exceptions;
using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Used to store any code that ends with ";"
/// </summary>
public class Instruction : IMember
{
    private ScriptFile parentFile;
    private IMember parent;
    private string body;
    private (int begin, int end) dimension;
    private InstructionParser parser;

    public ScriptFile ParentFile => parentFile;
    public GenericContext Context => throw new NotImplementedException();

    public List<SyntaxException> Exceptions => throw new NotImplementedException();

    public Instruction(string instructionContent, (int, int) dimension, IMember parent)
    {
        body = instructionContent;
        this.parent = parent;
        parentFile = parent.ParentFile;
        // Begin of given dimension is first character after end of previous instruction/block. This value need refining.
        // End is ";", as otherwise no instruction could be created at first place. This value can be taken as is.
        this.dimension = dimension;

        // Iterate through first characters in body to find first non-empty character. It's position should be taken as beginning.
        for (int i = 0; i < instructionContent.Length; i++)
        {
            if (!char.IsWhiteSpace(instructionContent[i]))
            {
                // It's first non-empty character at the body's beginning. It should be begin position.
                this.dimension.begin += i;
                break;
            }
        }

        // parser = new(body, dimension, this);
    }

    public void SplitContent()
    {

    }

    public override string ToString()
    {
        string result = "";
        (int line, int column) start = parentFile.IndexToCoordinates(dimension.begin);
        (int line, int column) end = parentFile.IndexToCoordinates(dimension.end);
        result += $"s: {dimension.begin}, e: {dimension.end}; l: {start.line}, c: {start.column}; l: {end.line}, c: {end.column} -> ";
        result += body.Replace('\n', '\0').Trim();
        return result;
    }

    public List<SyntaxException> CollectExceptions()
    {
        return [];
    }
}