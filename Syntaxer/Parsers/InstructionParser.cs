using Syntaxer.Context;
using Syntaxer.Enumerations;
using Syntaxer.Exceptions;
using Syntaxer.Members;
using Syntaxer.Operations;

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
        if (bodyElements.IndexOf(Keywords.NAMESPACE) != 0)
        {
            // Namespace keyword is not first.
            // Throw an error.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.KEYWORD_IS_NOT_FIRST_MESSAGE));
            return;
        }
        MemberType locationType = parent.Parent.Context.MemberType;
        if (locationType != MemberType.File && locationType != MemberType.Namespace)
        {
            // Namespace placed in the wrong location.
            // Throw an error.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.INCORRECT_PLACE_MESSAGE));
            return;
        }

        // At this point, there is only one namespace word, and it is first word in the sequece.
        List<string> leftover = bodyElements[1..];
        if (leftover.Count == 0)
        {
            // There is no name provided.
            // Throw an error.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.NO_NAME_DECLARED_MESSAGE));
            return;
        }

        // Parse long name.
        LongNameParser longNameParser = new(leftover, dimension);
        List<AccessOperation> names = longNameParser.ParseBody();
        foreach (var name in names)
        {
            name.Validate();
            exceptions.AddRange(name.Exceptions);
        }
        if (names.Count != 1)
        {
            // Throw exception about wrong naming.
            exceptions.Add(new NamespaceDeclarationException(dimension.begin, NamespaceDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
            return;
        }
    }

    public GenericContext ParseBody()
    {
        bodyElements = SplitBody();
        GenericContext contextToReturn;
        if (bodyElements.Contains(Keywords.NAMESPACE))
        {
            HandleNamespaceChecks();
            contextToReturn = new GenericContext(MemberType.Namespace);
        }
        else if (bodyElements.Contains(Keywords.CLASS))
        {
            contextToReturn = new GenericContext(MemberType.Class);
        }
        else if (bodyElements.Contains(Keywords.INTERFACE))
        {
            contextToReturn = new GenericContext(MemberType.Interface);
        }
        else if (bodyElements.Contains(Keywords.ENUM))
        {
            contextToReturn = new GenericContext(MemberType.Enum);
        }
        else
        {
            contextToReturn = new(MemberType.Unknown);
        }
        return contextToReturn;
    }
}