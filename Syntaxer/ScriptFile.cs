namespace Syntax;

public class ScriptFile
{
    private readonly string body;
    private readonly List<object> members = [];
    private readonly List<int> fileSize = []; // Each index represents line, it's value represents length. 

    public ScriptFile(string body)
    {
        this.body = body;
        CalculateFileSize();
    }

    private void CalculateFileSize()
    {
        int column = 0;
        foreach (var symbol in body)
        {
            if (symbol == '\n')
            {
                fileSize.Add(column);
                column = 0;
            }
            else
            {
                column++;
            }
        }
    }

    public void ClearBody()
    {
        string cleanBody = "";
        for (int i = 0; i < body.Length; i++)
        {
            
        }
    }

    public void CreateBracketMap()
    {
        // Markers of cursor position.
        int line = 0;
        int column = 0;

        bool isChar = false;
        bool isString = false;
        // Helping variables for comment detection.
        char previousSymbol = '\0';
        bool isLineComment = false;
        bool isMultilineComment = false;

        string brackets = "";

        foreach (var symbol in body)
        {
            // Store two last symbols for some checks.
            string twoSymbols = $"{previousSymbol}{symbol}";
            previousSymbol = symbol;

            if (isChar)
            {
                // Cursor is currently in char sequence. Detect it's end.
                if (twoSymbols == "\'")
                {
                    // Special case, when char stores single quote -> '\''. Do nothing.
                    continue;
                }
                if (symbol == '\'')
                {
                    // End of char sequence.
                    isChar = false;
                    continue;
                }
            }
            if (isString)
            {
                // Cursor is currently in string sequence. Detect it's end.
                if (twoSymbols == "\\\"")
                {
                    // Special case, when string stores single quote -> '\''. Do nothing.
                    continue;
                }
                if (symbol == '\'')
                {
                    // End of char sequence.
                    isChar = false;
                    continue;
                }
            }

            // Traverse through important symbols
            if (symbol == '\n')
            {
                // Update cursor.
                line++;
                column = 0;
                continue;
            }
            if (symbol == '\'')
            {
                // Start of char detected.
                isChar = true;
                continue;
            }
            if (symbol == '"')
            {
                // Start of string detected.
                isString = true;
                continue;
            }



            column++;

            // Detect comment start.

            if (twoSymbols == "//")
            {
                // Line comment detected.
                isLineComment = true;
            }
            else if (twoSymbols == "/*")
            {
                // Multiline comment detected.
                isMultilineComment = true;
            }

            // Look for end of the comment (if there is one). Do nothing else, as comment's content is ignored.
            if (isLineComment)
            {
                if (symbol == '\n')
                {
                    // Line end is end of line comment.
                    isLineComment = false;
                }
                continue;
            }
            if (isMultilineComment)
            {
                if (twoSymbols == "*/")
                {
                    // This sequence is end of multiline comment.
                    isMultilineComment = false;
                }
                continue;
            }

            // Detect strings and chars to ignore them as well.

            // Add current symbol to current member.
            // member += symbol;
            // if (isScanningBlock)
            // {
            //     // Just pass through.
            //     continue;
            // }
            // if (symbol == ';')
            // {
            //     // End of instruction found.
            //     members.Add(new Instruction(member));
            //     member = "";
            // }
            // else if (symbol == '{')
            // {
            //     // Start of block found. Begin block scan.
            //     isScanningBlock = true;
            // }
        }
    }

    public void Split()
    {
        // Markers of cursor position.
        int line = 0;
        int column = 0;
        // Helping variables for comment detection.
        char previousSymbol = '\0';
        bool isLineComment = false;
        bool isMultilineComment = false;
        // Remember scanned text to write it as member, when one is found.
        string member = "";
        // Helping variables for traversing blocks.
        int bracketBalance = 0;
        bool isScanningBlock = false;
        // Separate body into members and calculate file size.
        foreach (var symbol in body)
        {
            // Count file size.
            if (symbol == '\n')
            {
                fileSize.Add(column);
                line++;
                column = 0;
            }
            else
            {
                column++;
            }

            // Detect comment start.
            string twoSymbols = $"{previousSymbol}{symbol}";
            if (twoSymbols == "//")
            {
                // Line comment detected.
                isLineComment = true;
            }
            else if (twoSymbols == "/*")
            {
                // Multiline comment detected.
                isMultilineComment = true;
            }

            // Look for end of the comment (if there is one). Do nothing else, as comment's content is ignored.
            if (isLineComment)
            {
                if (symbol == '\n')
                {
                    // Line end is end of line comment.
                    isLineComment = false;
                }
                continue;
            }
            if (isMultilineComment)
            {
                if (twoSymbols == "*/")
                {
                    // This sequence is end of multiline comment.
                    isMultilineComment = false;
                }
                continue;
            }

            // Add current symbol to current member.
            member += symbol;
            if (isScanningBlock)
            {
                // Just pass through.
                continue;
            }
            if (symbol == ';')
            {
                // End of instruction found.
                members.Add(new Instruction(member));
                member = "";
            }
            else if (symbol == '{')
            {
                // Start of block found. Begin block scan.
                isScanningBlock = true;
            }
        }
        fileSize.Add(column);
    }
}