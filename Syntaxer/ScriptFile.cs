using Syntax;

namespace Syntaxer;

public class ScriptFile
{
    private string originalBody;
    private List<string> linesOfFile = [];
    private List<object> members = [];

    public ScriptFile(string body)
    {
        originalBody = body;
        SplitFileIntoLines();
        SplitFileIntoMembers();
    }

    private void SplitFileIntoLines()
    {
        string line = "";
        foreach (var symbol in originalBody)
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

    private void SplitFileIntoMembers()
    {
        string member = "";

        for (int i = 0; i < originalBody.Length; i++)
        {
            if (originalBody[i] == '/')
            {
                // Possible comment.
                if (ScanUtils.IsLastSymbol(originalBody, i))
                {
                    // It's last symbol of the body. There can't be other / futher to form a comment.
                    member += originalBody[i];
                }
                else if (originalBody[i + 1] == '/')
                {
                    // It's a comment.
                    i = ScanUtils.SkipComment(originalBody, i);
                }
                continue;
            }
            else if (originalBody[i] == '\'')
            {
                // Char declarator found.
                i = ScanUtils.SkipString(originalBody, i, member, StringType.Char);
                continue;
            }
            else if (originalBody[i] == '"')
            {
                // String declarator found.
                i = ScanUtils.SkipString(originalBody, i, member, StringType.String);
                continue;
            }
            else if (originalBody[i] == ';')
            {
                // End of instruction found.
                member += originalBody[i];
                members.Add(new Instruction(member));
                member = "";
                continue;
            }
            else if (originalBody[i] == '{')
            {
                // Start of block found.
                i = ScanUtils.SkipBlock(originalBody, i, member);
                members.Add(new Block(member));
                member = "";
                continue;
            }
            member += originalBody[i];
        }
        
        foreach (var m in members)
        {
            Console.WriteLine(m);
        }
    }
}