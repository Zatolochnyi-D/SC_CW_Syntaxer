using Syntaxer.Context;
using Syntaxer.Enumerations;
using Syntaxer.Exceptions;
using Syntaxer.Members;
using Syntaxer.Operations;

namespace Syntaxer.Parsers;

public class IdentifierParser
{
    private string body;
    private string cleanBody;
    private List<string> bodyElements = [];
    private (int begin, int end) dimension;
    private List<SyntaxException> exceptions = [];
    private IMember parent;

    public List<SyntaxException> Exceptions => exceptions;

    // Content is given without ;
    public IdentifierParser(string instructionContent, (int, int) dimension, IMember parent)
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
                    if (Keywords.NON_ACCESS_OPERATORS.Contains(possibleOperator))
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

    private void HandleClassChecks()
    {
        MemberType locationType = parent.Parent.Context.MemberType;
        MemberType[] allowedTypes = [MemberType.File, MemberType.Namespace, MemberType.Class, MemberType.Interface];
        if (!allowedTypes.Contains(locationType))
        {
            // Class placed in the wrong location.
            exceptions.Add(new ClassDeclarationException(dimension.begin, NamespaceDeclarationException.INCORRECT_PLACE_MESSAGE));
            return;
        }

        int indexOfClassKeyword = bodyElements.IndexOf(Keywords.CLASS);
        if (indexOfClassKeyword - 1 != -1)
        {
            List<string> leftPart = bodyElements[..indexOfClassKeyword];
            List<string> wrongModifiers = bodyElements.Where(x => !Keywords.CLASS_MODIFIERS.Contains(x)).ToList();
            foreach (var word in wrongModifiers)
            {
                // There is some unrecognized words.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetUnrecognizedWordMessage(word)));
            }
            List<string> duplicates = leftPart.GroupBy(x => x).Where(group => group.Count() != 1).Select(x => x.Key).Distinct().ToList();
            foreach (var duplicate in duplicates)
            {
                // There is some duplicates.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetDuplicateWordMessage(duplicate)));
            }
            List<string> conflictKeywords = leftPart.Where(x => x == Keywords.ABSTRACT || x == Keywords.SEALED || x == Keywords.STATIC).Distinct().ToList();
            if (conflictKeywords.Count > 1)
            {
                // There are more than 1 conflict words, thus there is an conflict.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetConflictedMessage(conflictKeywords)));
            }
        }

        List<string> rightPart;
        if (indexOfClassKeyword + 1 != bodyElements.Count)
        {
            rightPart = bodyElements[(indexOfClassKeyword + 1)..];
            int indexOfInheritanceDeclarator = rightPart.IndexOf(Keywords.INHERITANCE_OPERATOR);
            if (indexOfInheritanceDeclarator != -1)
            {
                // There are inheritance part.
                if (indexOfInheritanceDeclarator - 1 != -1)
                {
                    List<string> name = rightPart[..indexOfInheritanceDeclarator];
                    if (name.Count != 1)
                    {
                        // There is to many words in name.
                        exceptions.Add(new ClassDeclarationException(dimension.begin, ClassDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
                    }
                }
            }
            else
            {
                // No inheritance, only name.
                if (rightPart.Count != 1)
                {
                    // There is to many words in name.
                    exceptions.Add(new ClassDeclarationException(dimension.begin, ClassDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
                }
            }
        }
        else
        {
            // There is nothing at right of keyword, meaning there is no name provided.
            exceptions.Add(new ClassDeclarationException(dimension.begin, ClassDeclarationException.NO_NAME_DECLARED_MESSAGE));
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
            HandleClassChecks();
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