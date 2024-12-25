
namespace Syntaxer.Members;

/// <summary>
/// Used to store any code that ends with ";"
/// </summary>
public class Instruction : IMember
{
    private ScriptFile parentFile;
    private IMember parentMember;
    private string body;
    private int beginPosition; // Position of first non-empty symbol in body.
    private int endPosition; // Position of last non-empty symbol in body (should be ;).

    public Instruction(string body, int beginPosition, int endPosition, ScriptFile parentFile, IMember parentMember)
    {
        this.body = body;
        this.parentFile = parentFile;
        this.parentMember = parentMember;

        for (int i = 0; i < body.Length; i++)
        {
            if (!char.IsWhiteSpace(body[i]))
            {
                // It's first non-empty character at the body's beginning. It should be begin position;
                this.beginPosition = beginPosition + i;
                break;
            }
        }
        for (int i = 0; i < body.Length; i++)
        {
            if (!char.IsWhiteSpace(body[^(i + 1)]))
            {
                // It's first non-empty character in end of the body. It should be end position;
                this.endPosition = endPosition - i;
                break;
            }
        }
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