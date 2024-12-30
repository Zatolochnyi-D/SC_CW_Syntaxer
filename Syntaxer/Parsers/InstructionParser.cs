using Syntaxer.Context;
using Syntaxer.Exceptions;
using Syntaxer.Members;

namespace Syntaxer.Parsers;

public class InstructionParser
{
    private string body;
    private string cleanBody;
    private List<string> bodyElements = [];
    private (int begin, int end) dimension;
    private List<SyntaxException> exceptions = [];
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
                if (word != string.Empty) words.Add(word);
                if (i != target.Length - 1)
                {
                    string possibleOperator = $"{target[i]}{target[i + 1]}";
                    if (Keywords.OPERATORS.Contains(possibleOperator))
                    {
                        // This and next symbol form 2-symbol operator.
                        words.Add(possibleOperator);
                        i++;
                        word = "";
                        continue;
                    }
                }
                words.Add(target[i].ToString());
                word = "";
                continue;
            }
            word += target[i];
        }
        if (word != string.Empty) words.Add(word);
        return words;
    }

    /// <summary>
    /// Split line into separate words or symbols for futher analysis.
    /// </summary>
    /// <returns>List of words and symbols.</returns>
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

    private void HandleNamespaceChecks()
    {
        // This body is a namespace.
        if (bodyElements.IndexOf(Keywords.NAMESPACE) != 0)
        {
            // Namespace keyword is not first. Throw error.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.KEYWORD_IS_NOT_FIRST_MESSAGE));
        }
        MemberType locationType = parent.Parent.Context.MemberType;
        if (locationType == MemberType.File || locationType == MemberType.Namespace)
        {
            // Namespace is located in proper place.
            string[] leftover = bodyElements.Where(x => x != Keywords.NAMESPACE).ToArray();
            if (leftover.Length == 0)
            {
                // There is no name provided.
                exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.ANY_NAME_DECLARED_MESSAGE));
            }

            int firstDotPosition = bodyElements.IndexOf(".");
            if (firstDotPosition == -1)
            {
                // There is no dots, than sequence should contain only 2 words.
                if (leftover.Length != 1)
                {
                    // Throw exception about wrong naming.
                    exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
                }
            }
            else
            {
                // Parse long name.

            }
        }
        else
        {
            // Throw incorrect place for namespace.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.INCORRECT_PLACE_MESSAGE));
        }
    }

    public void ParseBody()
    {
        bodyElements = SplitBody();
        foreach(var el in bodyElements)
        {
            Console.Write("  ");
            Console.Write(el);
        }
        Console.Write("\n");
        GenericContext contextToReturn;
        if (bodyElements.Contains(Keywords.NAMESPACE))
        {
            HandleNamespaceChecks();
        }
        else
        {
            contextToReturn = new(MemberType.Unknown);
        }
    }
}