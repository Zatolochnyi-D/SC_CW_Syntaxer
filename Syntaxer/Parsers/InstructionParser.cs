using Syntaxer.Context;
using Syntaxer.Members;

namespace Syntaxer.Parsers;

public class InstructionParser
{
    private string body;
    private string cleanBody;
    private List<string> bodyElements = [];
    private (int begin, int end) dimension;
    private IMember parent;

    // Content is given without ;
    public InstructionParser(string instructionContent, (int, int) dimension, IMember parent)
    {
        body = instructionContent;
        cleanBody = new string(body.Where(x => !char.IsControl(x)).ToArray()); // Remove invisible characters.
        this.dimension = dimension;
        this.parent = parent;
    }

    /// <summary>
    /// Reads string for symbols. Splits it into left word, found symbol and right word.
    /// </summary>
    /// <param name="target">String to split.</param>
    /// <returns>List of splitted words.</returns>
    private List<string> SplitStringBySymbols(string target)
    {
        string word = "";
        List<string> words = [];
        for (int i = 0; i < target.Length; i++)
        {
            if (!char.IsLetterOrDigit(target[i]))
            {
                // TODO Check for 2-char operators!!!
                if (word != string.Empty) words.Add(word);
                words.Add(target[i].ToString());
                word = "";
                continue;
            }
            word += target[i];
        }
        if (word != string.Empty) words.Add(word);
        return words;
    }

    private List<string> SplitBody()
    {
        IEnumerable<string> splitResult = cleanBody.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        List<string> result = [];
        foreach (var element in splitResult)
        {
            result.AddRange(SplitStringBySymbols(element));
        }
        return result;
    }

    public GenericContext ParseBody()
    {
        bodyElements = SplitBody();
        GenericContext contextToReturn;
        if (bodyElements.Contains(Keywords.NAMESPACE))
        {
            // Look for errors.
            NamespaceContext context = new(MemberType.Namespace);
            contextToReturn = context;
        }
        else
        {
            contextToReturn = new(MemberType.Unknown);
        }
        return contextToReturn;
    }
}