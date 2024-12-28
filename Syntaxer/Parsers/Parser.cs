using Syntaxer.Enumerations;
using Syntaxer.Exceptions;
using Syntaxer.Members;

namespace Syntaxer.Parsers;

public abstract class Parser
{
    protected string Body;

    public Parser(string body)
    {
        Body = body;
    }

    /// <summary>
    /// Checks if current symbol is terminating by checking amount of backslashes before it.
    /// </summary>
    /// <param name="position">Position of cursor, where opening symbol was found.</param>
    /// <returns>True, if it's a termination symbol and not string (char) content.</returns>
    protected virtual bool IsStringTerminator(int position)
    {
        int i = 0; // Counter of amount of backslashes before possible terminator.
        while (true)
        {
            // Go back and count amount of backslashes before possible terminator.
            i++;
            if (Body[position - i] != '\\')
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
    protected virtual bool IsEndOfBody(int position)
    {
        return position == Body.Length;
    }

    /// <summary>
    /// Checks if position is the last symbol of the body.
    /// </summary>
    /// <param name="position">Position of cursor.</param>
    /// <returns>True, if position leads to the last symbol of the body.</returns>
    protected virtual bool IsLastSymbol(int position)
    {
        return position == Body.Length - 1;
    }

    /// <summary>
    /// Moves position cursor forward until end of comment found. Outputs space characters for each comment symbol to preserve length.
    /// </summary>
    /// <param name="position">Position of cursor, where first / (slash) of comment found.</param>
    /// <param name="readOutput">String to where output empty space.</param>
    /// <returns>Position where comment ends or last symbol of the body.</returns>
    protected virtual int SkipComment(int position, ref string readOutput)
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
        } while (Body[position] != '\n'); // It's an end of the comment.
        return position;
    }

    /// <summary>
    /// Skips comment without outputing empties for each comment character.
    /// </summary>
    /// <param name="position">Position where first / of comment was found.</param>
    /// <returns>Position where comment ends.</returns>
    protected virtual int SilentSkipComment(int position)
    {
        string message = "";
        return SkipComment(position, ref message);
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
    protected virtual int SkipString(int position, ref string readOutput, StringType type)
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
                throw new NotImplementedException($"Unknown type was given - {type}");
        }

        readOutput += Body[position];
        while (true)
        {
            // Move position forward and write contents. Look for termination symbol.
            position++;
            // Do not perform checks for end of file or line, as string correctness is checked before parsing.
            readOutput += Body[position];
            if (Body[position] == terminationSymbol) if (IsStringTerminator(position)) break;
        }
        return position;
    }

    /// <summary>
    /// Read and write block.
    /// </summary>
    /// <param name="position">Cursor position where block open bracket is found.</param>
    /// <param name="readOutput">String to where block contents should be written.</param>
    /// <returns>Position where block closing bracket or last symbol of body was found.</returns>
    protected virtual int SkipBlock(int position, ref string readOutput)
    {
        readOutput += Body[position];
        int bracketBalance = -1; // "{" counts as -1 and "}" as +1. That means "{ }" forms 0 together
        while (true)
        {
            position++;
            if (IsEndOfBody(position))
            {
                position--;
                break; // Nothing to scan futher.
            }
            if (Body[position] == '\'')
            {
                position = SkipString(position, ref readOutput, StringType.Char);
            }
            else if (Body[position] == '"')
            {
                position = SkipString(position, ref readOutput, StringType.String);
            }
            else
            {
                readOutput += Body[position];
            }

            if (Body[position] == '}') bracketBalance++;
            else if (Body[position] == '{') bracketBalance--;

            if (bracketBalance == 0) break;
        }
        return position;
    }
}