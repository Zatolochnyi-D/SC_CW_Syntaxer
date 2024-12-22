using Syntax;

namespace Syntaxer;

public class ScriptFile
{
    private string originalBody;
    private List<string> linesOfFile = [];
    private List<int> fileSize = []; // Each index represents line, it's value represents length. 
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
                if (originalBody[i + 1] == '/' && i != originalBody.Length - 1)
                {
                    // It's a comment.
                    while (true)
                    {
                        // Increment i until end of line or end of file found.
                        i++;
                        if (i == originalBody.Length)
                        {
                            break;
                        }
                        if (originalBody[i] == '\n')
                        {
                            break;
                        }
                    }
                    continue;
                }
            }
            else if (originalBody[i] == '\'')
            {
                // Char declarator found.
                member += originalBody[i];
                while (true)
                {
                    // As it's a char, go forward and read it completely. It ignores structure and treats everything as content of char.
                    i++;
                    if (i == originalBody.Length)
                    {
                        // Reached end of file.
                        break;
                    }
                    member += originalBody[i];
                    if (originalBody[i] == '\'')
                    {
                        int j = 0;
                        while (true)
                        {
                            // Go back and count amount of backslashes before char declarator.
                            j++;
                            if (originalBody[i - j] != '\\')
                            {
                                j--;
                                break;
                            }
                        }
                        if (j % 2 == 0)
                        {
                            // It's char end declarator.
                            break;
                        }
                    }
                }
                continue;
            }
            else if (originalBody[i] == '"')
            {
                // String declarator found.
                member += originalBody[i];
                while (true)
                {
                    // As it's a char, go forward and read it completely. It ignores structure and treats everything as content of char.
                    i++;
                    if (i == originalBody.Length)
                    {
                        // Reached end of file.
                        break;
                    }
                    member += originalBody[i];
                    if (originalBody[i] == '"')
                    {
                        int j = 0;
                        while (true)
                        {
                            // Go back and count amount of backslashes before string declarator.
                            j++;
                            if (originalBody[i - j] != '\\')
                            {
                                j--;
                                break;
                            }
                        }
                        if (j % 2 == 0)
                        {
                            // It's string end declarator.
                            break;
                        }
                    }
                }
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
                member += originalBody[i];
                int bracketBalance = -1;
                while (true)
                {
                    i++;
                    if (i == originalBody.Length)
                    {
                        // Reached end of file.
                        break;
                    }
                    member += originalBody[i];
                    if (originalBody[i] == '}')
                    {
                        bracketBalance++;
                    }
                    else if (originalBody[i] == '{')
                    {
                        bracketBalance--;
                    }
                    if (bracketBalance == 0)
                    {
                        break;
                    }
                }
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