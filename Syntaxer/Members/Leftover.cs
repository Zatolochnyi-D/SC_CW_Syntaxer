namespace Syntaxer.Members;

/// <summary>
/// Used for everything that left at the end of block parsing.
/// </summary>
public class Leftover : IMember
{
    private ScriptFile parentFile;
    private IMember parent;
    private string body;
    private (int begin, int end) dimension;

    public ScriptFile ParentFile => parentFile;

    public Leftover(string leftoverContent, (int, int) dimension, IMember parent)
    {
        body = leftoverContent;
        this.dimension = dimension;
        this.parent = parent;
        parentFile = parent.ParentFile;
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
}