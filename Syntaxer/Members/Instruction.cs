
namespace Syntaxer.Members;

/// <summary>
/// Used to store any code that ends with ";"
/// </summary>
public class Instruction : IMember
{
    private ScriptFile parentFile;
    private IMember parentMember;
    private string body;
    private int beginPosition;
    private int endPosition;

    public Instruction(string body, int beginPosition, int endPosition, ScriptFile parentFile, IMember parentMember)
    {
        this.body = body;
        this.beginPosition = beginPosition;
        this.endPosition = endPosition;
        this.parentFile = parentFile;
        this.parentMember = parentMember;
    }

    public void SplitContent()
    {

    }

    public override string ToString()
    {
        string result = "";
        (int line, int column) start = parentFile.IndexToCoordinates(beginPosition);
        (int line, int column) end = parentFile.IndexToCoordinates(endPosition);
        result += $"s: {beginPosition}, e: {endPosition}; l: {start.line}, c: {start.column}; l: {end.line}, c: {end.column} -> ";
        result += body.Replace('\n', '\0').Trim();
        return result;
    }
}