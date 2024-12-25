using Syntaxer.Members;

namespace Syntaxer.Parsers;

public enum StringType
{
    String,
    Char,
}

public class BlockParser
{
    private ScriptFile parentFile;
    private IMember parentMember;
    private string body;
    private int beginPosition;
    private int endPosition;

    public BlockParser(string body, int beginPosition, int endPosition, ScriptFile parentFile, IMember parentMember)
    {
        this.body = body;
        this.beginPosition = beginPosition;
        this.endPosition = endPosition;
        this.parentFile = parentFile;
        this.parentMember = parentMember;
    }

    /// <summary>
    /// Checks if current symbol is terminating by checking amount of backslashes before it.
    /// </summary>
    /// <param name="position">Position of cursor, where opening symbol was found.</param>
    /// <returns>True, if it's a termination symbol and not string (char) content.</returns>
    public bool IsStringTerminator(int position)
    {
        int i = 0; // Counter of amount of backslashes before possible terminator.
        while (true)
        {
            // Go back and count amount of backslashes before possible terminator.
            i++;
            if (body[position - i] != '\\')
            {
                i--;
                break;
            }
        }
        return i % 2 == 0; // If counter is even (like \\\\"), than it's a terminator (example: "\\\"" (\" content) and "\\\\"" (\\ content, one " is redundant)).
    }

    /// <summary>
    /// Checks if position is no longer inside the body (when position is or greater than body's length).
    /// </summary>
    /// <param name="position">Position of cursor.</param>
    /// <returns>True, if position is no longer inside of the body.</returns>
    public bool IsEndOfBody(int position)
    {
        return position == body.Length;
    }

    /// <summary>
    /// Checks if position is the last symbol of the body.
    /// </summary>
    /// <param name="position">Position of cursor.</param>
    /// <returns>True, if position leads to the last symbol of the body.</returns>
    public bool IsLastSymbol(int position)
    {
        return position == body.Length - 1;
    }

    /// <summary>
    /// Moves position cursor forward until end of comment found.
    /// </summary>
    /// <param name="position">Position of cursor, where first / (slash) of comment found.</param>
    /// <returns>Position where comment ends.</returns>
    public int SkipComment(int position)
    {
        do
        {
            // Move position forward.
            position++;
            if (IsEndOfBody(position)) break; // Nothing to scan futher.
        } while (body[position] != '\n'); // It's an end of the comment.
        return position;
    }

    /// <summary>
    /// Reads string or char until end of body or end of string (char) found. Writes all it read.
    /// </summary>
    /// <param name="position">Position of cursor, where opening symbol was found.</param>
    /// <param name="readOutput">String to where string (char) content should be appended.</param>
    /// <param name="type">Is it string or char.</param>
    /// <returns>Position of string (char) terminator.</returns>
    public int SkipString(int position, ref string readOutput, StringType type)
    {
        char terminationSymbol;
        switch (type)
        {
            case StringType.String:
                terminationSymbol = '"';
                break;
            case StringType.Char:
                terminationSymbol = '\'';
                break;
            default:
                throw new ArgumentException($"Unknown type was given - {type}");
        }

        readOutput += body[position];
        while (true)
        {
            // Move position forward and write contents. Look for termination symbol.
            position++;
            if (IsEndOfBody(position)) break; // Nothing to scan futher.
            readOutput += body[position];
            if (body[position] == terminationSymbol) if (IsStringTerminator(position)) break;
        }
        return position;
    }

    /// <summary>
    /// Read and write block.
    /// </summary>
    /// <param name="position">Cursor position where block open bracket is found.</param>
    /// <param name="readOutput">String to where block contents should be written.</param>
    /// <returns>Position where block closing bracket was found.</returns>
    public int SkipBlock(int position, ref string readOutput)
    {
        readOutput += body[position];
        int bracketBalance = -1; // "{" counts as -1 and "}" as +1. That means "{ }" forms 0 together
        while (true)
        {
            position++;
            if (IsEndOfBody(position)) break; // Nothing to scan futher.
            readOutput += body[position];

            if (body[position] == '}') bracketBalance++;
            else if (body[position] == '{') bracketBalance--;

            if (bracketBalance == 0) break;
        }
        return position;
    }

    public List<IMember> ParseBody()
    {
        string member = "";
        List<IMember> members = [];
        int startPosition = beginPosition;

        for (int i = 0; i < body.Length; i++)
        {
            if (body[i] == '/')
            {
                // Possible comment.
                if (IsLastSymbol(i))
                {
                    // It's last symbol of the body. There can't be other / futher to form a comment.
                    member += body[i];
                }
                else if (body[i + 1] == '/')
                {
                    // It's a comment.
                    i = SkipComment(i);
                }
                else
                {
                    member += body[i];
                }
                continue;
            }
            else if (body[i] == '\'')
            {
                // Char declarator found.
                i = SkipString(i, ref member, StringType.Char);
                continue;
            }
            else if (body[i] == '"')
            {
                // String declarator found.
                i = SkipString(i, ref member, StringType.String);
                continue;
            }
            else if (body[i] == ';')
            {
                // End of instruction found.
                member += body[i];
                members.Add(new Instruction(member, startPosition, beginPosition + i, parentFile, parentMember));
                startPosition = beginPosition + i + 1;
                member = "";
                continue;
            }
            else if (body[i] == '{')
            {
                // Start of block found.
                i = SkipBlock(i, ref member);
                members.Add(new Block(member, startPosition, beginPosition + i, parentFile, parentMember));
                startPosition = beginPosition + i + 1;
                member = "";
                continue;
            }
            member += body[i];
        }
        if (!member.All(char.IsWhiteSpace))
        {
            // Do not create leftover if string is empty actually.
            members.Add(new Leftover(member, startPosition, beginPosition + body.Length - 1, parentFile, parentMember));
        }
        return members;
    }
}