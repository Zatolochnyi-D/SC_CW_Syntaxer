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

    public List<SyntaxException> Exceptions => exceptions;

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
        if (bodyElements.Count(x => x == Keywords.NAMESPACE) != 1)
        {
            // There is more than 1 namespace keywords were found.
            // Throw non-critical error. It should prevent identifier from scanning futher, but should not prevent block from scanning.
            exceptions.Add(new KeywordMisusageException(dimension.begin, KeywordMisusageException.GetFoundDuplicateMessage(Keywords.NAMESPACE)));
            return;
        }
        if (bodyElements.IndexOf(Keywords.NAMESPACE) != 0)
        {
            // Namespace keyword is not first.
            // Throw non-critical error.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.KEYWORD_IS_NOT_FIRST_MESSAGE));
            return;
        }

        MemberType locationType = parent.Parent.Context.MemberType;
        if (locationType != MemberType.File && locationType != MemberType.Namespace)
        {
            // Namespace placed in the wrong location.
            // Throw non-critical error.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.INCORRECT_PLACE_MESSAGE));
            return;
        }

        // At this point, there is only one namespace word, and it is first word in the sequece.
        List<string> leftover = bodyElements.Where(x => x != Keywords.NAMESPACE).ToList();
        if (leftover.Count == 0)
        {
            // There is no name provided.
            // Throw non-critical error.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.ANY_NAME_DECLARED_MESSAGE));
            return;
        }

        int firstDotPosition = bodyElements.IndexOf(".");
        if (firstDotPosition == -1)
        {
            // There is no dots, than sequence should contain only 2 words.
            if (leftover.Count != 1)
            {
                // Throw exception about wrong naming.
                exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
                return;
            }
        }
        else
        {
            // Parse long name.
            LongNameParser longNameParser = new(leftover, dimension);
            List<string>? names = longNameParser.ParseBody();
            if (names == null)
            {
                // An error occured.
                exceptions.AddRange(longNameParser.Exceptions);
                return;
            }
            else if (names.Count != 1)
            {
                // Throw exception about wrong naming.
                exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
                return;
            }
        }
    }

    public GenericContext ParseBody()
    {
        bodyElements = SplitBody();
        GenericContext contextToReturn;
        if (bodyElements.Contains(Keywords.NAMESPACE))
        {
            HandleNamespaceChecks();
            contextToReturn = new NamespaceContext();
        }
        else
        {
            contextToReturn = new(MemberType.Unknown);
        }
        return contextToReturn;
    }
}