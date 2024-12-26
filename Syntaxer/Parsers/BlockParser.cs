using Syntaxer.Exceptions;
using Syntaxer.Members;

namespace Syntaxer.Parsers;

public enum StringType
{
    String,
    Char,
}

public class BlockParser
{
    private IMember parent;
    private string body;
    private (int begin, int end) dimension;

    public BlockParser(string body, (int, int) dimension, IMember parent)
    {
        this.body = body;
        this.dimension = dimension;
        this.parent = parent;
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
    /// Moves position cursor forward until end of comment found. Outputs space characters for each comment symbol to preserve length.
    /// </summary>
    /// <param name="position">Position of cursor, where first / (slash) of comment found.</param>
    /// <param name="readOutput">String to where output empty space.</param>
    /// <returns>Position where comment ends or last symbol of the body.</returns>
    public int SkipComment(int position, ref string readOutput)
    {
        readOutput += ' ';
        do
        {
            // Move position forward.
            position++;
            if (IsEndOfBody(position))
            {
                position--; // So method returns last symbol.
                break; // Nothing to scan futher.
            }
            readOutput += ' ';
        } while (body[position] != '\n'); // It's an end of the comment.
        return position;
    }

    /// <summary>
    /// Reads string or char until end of body or end of string (char) found. Writes all it read.
    /// </summary>
    /// <param name="position">Position of cursor, where opening symbol was found.</param>
    /// <param name="readOutput">String to where string (char) content should be appended.</param>
    /// <param name="type">Is it string or char.</param>
    /// <returns>Position where string end (string termionator or last symbol of the file).</returns>
    /// <exception cref="NotImplementedException">Throws exception if nes StringType is added but reaction on it is not implemented.</exception>
    /// <exception cref="OpenStringException">Happens when all body was scanned, but end of string was not found.</exception>
    public int SkipString(int position, ref string readOutput, StringType type)
    {
        int start = position;
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
                throw new NotImplementedException($"Unknown type was given - {type}");
        }

        readOutput += body[position];
        while (true)
        {
            // Move position forward and write contents. Look for termination symbol.
            position++;
            if (IsEndOfBody(position) || body[position] == '\n')
            {
                // That means the string was not closed. Futher scan is impossible.
                throw new OpenStringException("End of the string was not found", start);
            }
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
    /// <returns>Position where block closing bracket or last symbol of body was found.</returns>
    public int SkipBlock(int position, ref string readOutput)
    {
        readOutput += body[position];
        int bracketBalance = -1; // "{" counts as -1 and "}" as +1. That means "{ }" forms 0 together
        while (true)
        {
            position++;
            if (IsEndOfBody(position))
            {
                position--;
                break; // Nothing to scan futher.
            }
            if (body[position] == '\'')
            {
                position = SkipString(position, ref readOutput, StringType.Char);
            }
            else if (body[position] == '"')
            {
                position = SkipString(position, ref readOutput, StringType.String);
            }
            else
            {
                readOutput += body[position];
            }

            if (body[position] == '}') bracketBalance++;
            else if (body[position] == '{') bracketBalance--;

            if (bracketBalance == 0) break;
        }
        return position;
    }

    public List<IMember> ParseBody()
    {
        string member = "";
        int start = dimension.begin;
        List<IMember> members = [];

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
                    i = SkipComment(i, ref member);
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
                int end = dimension.begin + i; // Points at ";". "i" is local position, shift by begin to convert to file postion.
                members.Add(new Instruction(member, (start, end), parent));
                start = dimension.begin + i + 1; // Next symbol becomes start of next instruction/block.
                member = "";
                continue;
            }
            else if (body[i] == '{')
            {
                // Start of block found.
                i = SkipBlock(i, ref member); // This i either position of "}" or last symbol of the file.
                int end = dimension.begin + i;
                members.Add(new Block(member, (start, end), parent));
                start = dimension.begin + i + 1;
                member = "";
                continue;
            }
            member += body[i];
        }
        // Leftover is created, if after previous block/instruction there is still symbols, and those symbols are not any whitespace characters.
        if (!member.All(char.IsWhiteSpace))
        {
            // Do not create leftover if string is empty actually.
            members.Add(new Leftover(member, (start, dimension.begin + body.Length - 1), parent));
        }
        return members;
    }
}