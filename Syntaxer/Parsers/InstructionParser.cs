using Syntaxer.Members;

namespace Syntaxer.Parsers;

public class InstructionParser
{
    private string body;
    private string[] bodyElements;

    public InstructionParser(string instructionContent, (int, int) dimension, IMember parent)
    {
        body = instructionContent;
        bodyElements = body.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        foreach (var el in bodyElements)
        {
            Console.WriteLine($"\t {el}");
        }
    }
}