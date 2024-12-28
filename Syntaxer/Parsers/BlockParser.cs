using Syntaxer.Enumerations;
using Syntaxer.Members;

namespace Syntaxer.Parsers;

public class BlockParser : Parser
{
    private IMember parent;
    private (int begin, int end) dimension;

    public BlockParser(string body, (int, int) dimension, IMember parent) : base(body)
    {
        this.dimension = dimension;
        this.parent = parent;
    }

    public List<IMember> ParseBody()
    {
        string member = "";
        int start = dimension.begin;
        List<IMember> members = [];

        for (int i = 0; i < Body.Length; i++)
        {
            if (Body[i] == '/')
            {
                // Possible comment.
                if (IsLastSymbol(i))
                {
                    // It's last symbol of the Body. There can't be other / futher to form a comment.
                    member += Body[i];
                }
                else if (Body[i + 1] == '/')
                {
                    // It's a comment.
                    i = SkipComment(i, ref member);
                }
                else
                {
                    member += Body[i];
                }
                continue;
            }
            else if (Body[i] == '\'')
            {
                // Char declarator found.
                i = SkipString(i, ref member, StringType.Char);
                continue;
            }
            else if (Body[i] == '"')
            {
                // String declarator found.
                i = SkipString(i, ref member, StringType.String);
                continue;
            }
            else if (Body[i] == ';')
            {
                // End of instruction found.
                member += Body[i];
                int end = dimension.begin + i; // Points at ";". "i" is local position, shift by begin to convert to file postion.
                members.Add(new Instruction(member, (start, end), parent));
                start = dimension.begin + i + 1; // Next symbol becomes start of next instruction/block.
                member = "";
                continue;
            }
            else if (Body[i] == '{')
            {
                // Start of block found.
                i = SkipBlock(i, ref member); // This i either position of "}" or last symbol of the file.
                int end = dimension.begin + i;
                members.Add(new Block(member, (start, end), parent));
                start = dimension.begin + i + 1;
                member = "";
                continue;
            }
            member += Body[i];
        }
        // Leftover is created, if after previous block/instruction there is still symbols, and those symbols are not any whitespace characters.
        if (!member.All(char.IsWhiteSpace))
        {
            // Do not create leftover if string is empty actually.
            members.Add(new Leftover(member, (start, dimension.begin + Body.Length - 1), parent));
        }
        return members;
    }
}