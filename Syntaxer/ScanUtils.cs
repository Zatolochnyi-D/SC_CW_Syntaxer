namespace Syntaxer;

public enum StringType
{
    String,
    Char,
}

public static class ScanUtils
{
    /// <summary>
    /// Moves position cursor forward until end of comment found.
    /// </summary>
    /// <param name="body">String on which scan is performed.</param>
    /// <param name="position">Position of cursor, where first / (slash) of comment found.</param>
    /// <returns>Position where comment ends.</returns>
    public static int SkipComment(string body, int position)
    {
        do
        {
            // Move position forward.
            position++;
            if (IsEndOfBody(body, position)) break; // Nothing to scan futher.
        } while (body[position] == '\n'); // It's an end of the comment.
        return position;
    }

    /// <summary>
    /// Reads string or char until end of body or end of string (char) found. Writes all it read.
    /// </summary>
    /// <param name="body">String on which scan is performed.</param>
    /// <param name="position">Position of cursor, where opening symbol was found.</param>
    /// <param name="readOutput">String to where string (char) content should be appended.</param>
    /// <param name="type">Is it string or char.</param>
    /// <returns>Position of string (char) termintor.</returns>
    public static int SkipString(string body, int position, string readOutput, StringType type)
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
            if (IsEndOfBody(body, position)) break; // Nothing to scan futher.
            readOutput += body[position];
            if (body[position] == terminationSymbol) if (IsStringTerminator(body, position)) break;
        }
        return position;
    }

    /// <summary>
    /// Checks if current symbol is terminating by checking amount of backslashes before it.
    /// </summary>
    /// <param name="body">String on which scan is performed.</param>
    /// <param name="position">Position of cursor, where opening symbol was found.</param>
    /// <returns>True, if it's a termination symbol and not string (char) content.</returns>
    public static bool IsStringTerminator(string body, int position)
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
    /// <param name="body">String on which scan is performed.</param>
    /// <param name="position">Position of cursor.</param>
    /// <returns>True, if position is no longer inside of the body.</returns>
    public static bool IsEndOfBody(string body, int position)
    {
        if (position == body.Length)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if position is the last symbol of the body.
    /// </summary>
    /// <param name="body">String on which scan is performed.</param>
    /// <param name="position">Position of cursor.</param>
    /// <returns>True, if position leads to the last symbol of the body.</returns>
    public static bool IsLastSymbol(string body, int position)
    {
        if (position == body.Length - 1)
        {
            return true;
        }
        return false;
    }
}