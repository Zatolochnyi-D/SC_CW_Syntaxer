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
        EnumerationParser enumerationParser = new(leftover, dimension, ["."]);
        List<Operation> names = enumerationParser.ParseBody();
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
            exceptions.Add(new TypePlaceException(dimension.begin));
        }

        int indexOfClassKeyword = bodyElements.IndexOf(Keywords.CLASS);
        if (indexOfClassKeyword - 1 != -1)
        {
            // There is something on left.
            List<string> leftPart = bodyElements[..indexOfClassKeyword];
            IEnumerable<string> wrongModifiers = leftPart.Where(x => !Keywords.CLASS_MODIFIERS.Contains(x));
            IEnumerable<string> nonModifiers = wrongModifiers.Where(x => !Keywords.ALL_KEYWORDS.Contains(x));
            wrongModifiers = wrongModifiers.Except(nonModifiers);
            foreach (var word in nonModifiers)
            {
                // Those words are not modifiers.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetNonModifierMessage(word)));
            }
            foreach (var modifier in wrongModifiers)
            {
                // There is some unrecognized words.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetWrongContextMessage(modifier)));
            }
            IEnumerable<string> correctModifiers = leftPart.Where(x => !wrongModifiers.Contains(x));
            IEnumerable<string> duplicates = correctModifiers.GroupBy(x => x).Where(group => group.Count() != 1).Select(x => x.Key).Distinct();
            foreach (var duplicate in duplicates)
            {
                // There is some duplicates.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetDuplicateWordMessage(duplicate)));
            }
            IEnumerable<string> conflictKeywords = correctModifiers.Where(x => x == Keywords.ABSTRACT || x == Keywords.SEALED || x == Keywords.STATIC).Distinct();
            if (conflictKeywords.Count() > 1)
            {
                // There are more than 1 conflict words, thus there is an conflict.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetConflictedMessage(conflictKeywords)));
            }
        }

        List<string> name = [];
        List<string>? inheritance = null;
        if (indexOfClassKeyword + 1 != bodyElements.Count)
        {
            // There is something on right.
            List<string> rightPart;
            rightPart = bodyElements[(indexOfClassKeyword + 1)..];
            int indexOfInheritanceDeclarator = rightPart.IndexOf(Keywords.INHERITANCE_OPERATOR);
            if (indexOfInheritanceDeclarator != -1)
            {
                // There are inheritance part.
                if (indexOfInheritanceDeclarator - 1 != -1)
                {
                    // There are something on left from inheritance operator (name presumably).
                    name = rightPart[..indexOfInheritanceDeclarator];
                    if (indexOfInheritanceDeclarator + 1 != rightPart.Count)
                    {
                        // There is inheritance on right side from inheritance operator.
                        inheritance = rightPart[(indexOfInheritanceDeclarator + 1)..];
                    }
                    else
                    {
                        inheritance = [];
                    }
                }
            }
            else
            {
                // No inheritance, only name.
                name = rightPart;
            }
        }

        if (name.Count == 0)
        {
            // Name is empty.
            exceptions.Add(new ClassDeclarationException(dimension.begin, ClassDeclarationException.NO_NAME_DECLARED_MESSAGE));
        }
        else if (name.Count != 1)
        {
            // To many words.
            exceptions.Add(new ClassDeclarationException(dimension.begin, ClassDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
        }
        else if (Keywords.ALL_KEYWORDS.Contains(name[0]))
        {
            // Provided name is a keyword.
            exceptions.Add(new ClassDeclarationException(dimension.begin, ClassDeclarationException.GetKeywordAsNameMessage(name[0])));
        }

        if (inheritance != null)
        {
            // Inheritance part is present
            if (inheritance.Count == 0)
            {
                exceptions.Add(new ClassDeclarationException(dimension.begin, ClassDeclarationException.INHERITANCE_MISSING_MESSAGE));
            }
            else
            {
                var enumParser = new EnumerationParser(inheritance, dimension, [","]);
                List<Operation> operations = enumParser.ParseBody();
                if (operations.Count != 1)
                {
                    // There is more than 1 operation, meaning , was missed.
                    exceptions.Add(new ClassDeclarationException(dimension.begin, ClassDeclarationException.COMMA_MISSED_IN_INHERITANCE));
                }
                else
                {
                    foreach (var operation in operations)
                    {
                        operation.Validate();
                        exceptions.AddRange(operation.Exceptions);
                    }
                }
            }
        }
    }

    private void HandleInterfaceChecks()
    {
        MemberType locationType = parent.Parent.Context.MemberType;
        MemberType[] allowedTypes = [MemberType.File, MemberType.Namespace, MemberType.Class, MemberType.Interface];
        if (!allowedTypes.Contains(locationType))
        {
            // Class placed in the wrong location.
            exceptions.Add(new TypePlaceException(dimension.begin));
        }

        int indexOfClassKeyword = bodyElements.IndexOf(Keywords.INTERFACE);
        if (indexOfClassKeyword - 1 != -1)
        {
            // There is something on left.
            List<string> leftPart = bodyElements[..indexOfClassKeyword];
            IEnumerable<string> wrongModifiers = leftPart.Where(x => !Keywords.ACCESS_MODIFIERS.Contains(x));
            IEnumerable<string> nonModifiers = wrongModifiers.Where(x => !Keywords.ALL_KEYWORDS.Contains(x));
            wrongModifiers = wrongModifiers.Except(nonModifiers);
            foreach (var word in nonModifiers)
            {
                // Those words are not modifiers.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetNonModifierMessage(word)));
            }
            foreach (var modifier in wrongModifiers)
            {
                // There is some unrecognized words.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetWrongContextMessage(modifier)));
            }
            IEnumerable<string> correctModifiers = leftPart.Where(x => !wrongModifiers.Contains(x));
            IEnumerable<string> duplicates = correctModifiers.GroupBy(x => x).Where(group => group.Count() != 1).Select(x => x.Key).Distinct();
            foreach (var duplicate in duplicates)
            {
                // There is some duplicates.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetDuplicateWordMessage(duplicate)));
            }
        }

        List<string> name = [];
        List<string>? inheritance = null;
        if (indexOfClassKeyword + 1 != bodyElements.Count)
        {
            // There is something on right.
            List<string> rightPart;
            rightPart = bodyElements[(indexOfClassKeyword + 1)..];
            int indexOfInheritanceDeclarator = rightPart.IndexOf(Keywords.INHERITANCE_OPERATOR);
            if (indexOfInheritanceDeclarator != -1)
            {
                // There are inheritance part.
                if (indexOfInheritanceDeclarator - 1 != -1)
                {
                    // There are something on left from inheritance operator (name presumably).
                    name = rightPart[..indexOfInheritanceDeclarator];
                    if (indexOfInheritanceDeclarator + 1 != rightPart.Count)
                    {
                        // There is inheritance on right side from inheritance operator.
                        inheritance = rightPart[(indexOfInheritanceDeclarator + 1)..];
                    }
                    else
                    {
                        inheritance = [];
                    }
                }
            }
            else
            {
                // No inheritance, only name.
                name = rightPart;
            }
        }

        if (name.Count == 0)
        {
            // Name is empty.
            exceptions.Add(new InterfaceDeclarationException(dimension.begin, InterfaceDeclarationException.NO_NAME_DECLARED_MESSAGE));
        }
        else if (name.Count != 1)
        {
            // To many words.
            exceptions.Add(new InterfaceDeclarationException(dimension.begin, InterfaceDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
        }
        else if (Keywords.ALL_KEYWORDS.Contains(name[0]))
        {
            // Provided name is a keyword.
            exceptions.Add(new InterfaceDeclarationException(dimension.begin, InterfaceDeclarationException.GetKeywordAsNameMessage(name[0])));
        }

        if (inheritance != null)
        {
            // Inheritance part is present
            if (inheritance.Count == 0)
            {
                exceptions.Add(new InterfaceDeclarationException(dimension.begin, InterfaceDeclarationException.INHERITANCE_MISSING_MESSAGE));
            }
            else
            {
                var enumParser = new EnumerationParser(inheritance, dimension, [","]);
                List<Operation> operations = enumParser.ParseBody();
                if (operations.Count != 1)
                {
                    // There is more than 1 operation, meaning , was missed.
                    exceptions.Add(new InterfaceDeclarationException(dimension.begin, InterfaceDeclarationException.COMMA_MISSED_IN_INHERITANCE));
                }
                else
                {
                    foreach (var operation in operations)
                    {
                        operation.Validate();
                        exceptions.AddRange(operation.Exceptions);
                    }
                }
            }
        }
    }

    private void HandleEnumChecks()
    {
        MemberType locationType = parent.Parent.Context.MemberType;
        MemberType[] allowedTypes = [MemberType.File, MemberType.Namespace, MemberType.Class, MemberType.Interface];
        if (!allowedTypes.Contains(locationType))
        {
            // Class placed in the wrong location.
            exceptions.Add(new TypePlaceException(dimension.begin));
        }

        int indexOfEnumKeyword = bodyElements.IndexOf(Keywords.ENUM);
        if (indexOfEnumKeyword - 1 != -1)
        {
            // There is something on left.
            List<string> leftPart = bodyElements[..indexOfEnumKeyword];
            IEnumerable<string> wrongModifiers = leftPart.Where(x => !Keywords.ACCESS_MODIFIERS.Contains(x));
            IEnumerable<string> nonModifiers = wrongModifiers.Where(x => !Keywords.ALL_KEYWORDS.Contains(x));
            wrongModifiers = wrongModifiers.Except(nonModifiers);
            foreach (var word in nonModifiers)
            {
                // Those words are not modifiers.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetNonModifierMessage(word)));
            }
            foreach (var modifier in wrongModifiers)
            {
                // There is some unrecognized words.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetWrongContextMessage(modifier)));
            }
            IEnumerable<string> correctModifiers = leftPart.Where(x => !wrongModifiers.Contains(x));
            IEnumerable<string> duplicates = correctModifiers.GroupBy(x => x).Where(group => group.Count() != 1).Select(x => x.Key).Distinct();
            foreach (var duplicate in duplicates)
            {
                // There is some duplicates.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetDuplicateWordMessage(duplicate)));
            }
        }

        List<string> name = [];
        if (indexOfEnumKeyword + 1 != bodyElements.Count)
        {
            name = bodyElements[(indexOfEnumKeyword + 1)..];
            // There is something on right.
        }

        if (name.Count == 0)
        {
            // Name is empty.
            exceptions.Add(new EnumDeclarationException(dimension.begin, EnumDeclarationException.NO_NAME_DECLARED_MESSAGE));
        }
        else if (name.Count != 1)
        {
            // To many words.
            exceptions.Add(new EnumDeclarationException(dimension.begin, EnumDeclarationException.MANY_NAMES_DECLARED_MESSAGE));
        }
        else if (Keywords.ALL_KEYWORDS.Contains(name[0]))
        {
            // Provided name is a keyword.
            exceptions.Add(new EnumDeclarationException(dimension.begin, EnumDeclarationException.GetKeywordAsNameMessage(name[0])));
        }
    }

    private MemberType IdentifyMethodType()
    {
        MemberType locationType = parent.Parent.Context.MemberType;
        MemberType[] allowedTypes = [MemberType.Class];
        if (!allowedTypes.Contains(locationType))
        {
            // Class placed in the wrong location.
            exceptions.Add(new TypePlaceException(dimension.begin));
        }

        int indexOfDestructorSymbol = bodyElements.IndexOf("~");
        if (indexOfDestructorSymbol != -1)
        {
            // Its an destructor.
            return MemberType.Destructor;
        }

        int indexOfOpenBracket = bodyElements.IndexOf("(");
        List<string> body = bodyElements[..indexOfOpenBracket];
        int amountOfWords = body.Where(x => !Keywords.ALL_KEYWORDS.Contains(x)).Count();
        if (amountOfWords == 1)
        {
            // Constructor.
            return MemberType.Constructor;
        }
        else if (amountOfWords == 2)
        {
            // Method.
            return MemberType.Method;
        }
        else
        {
            // Cannot be identified.
            return MemberType.Unknown;
        }
    }

    private void HandleDestructorChecks()
    {
        int indexOfOpenBracket = bodyElements.IndexOf("(");
        List<string> body = bodyElements[..indexOfOpenBracket];
        if (body.Any(Keywords.ALL_KEYWORDS.Contains))
        {
            // Destructor have modifiers but any allowed.
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.DESTRUCTOR_KEYWORD_FOUND));
        }
        int amountOfWords = body.Where(x => !Keywords.ALL_KEYWORDS.Contains(x)).Where(x => x != "~").Count();
        if (amountOfWords == 0)
        {
            // There is no name.
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.NO_NAME_FOUND));
        }
        else if (amountOfWords > 1)
        {
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.TO_MANY_WORDS));
        }

        int indexOfLastBracket = bodyElements.IndexOf(")");
        List<string> parameterContent = bodyElements[(indexOfOpenBracket + 1)..indexOfLastBracket];
        if (parameterContent.Count != 0)
        {
            // There are some parameters, what is not allowed.
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.DESTRUCTOR_PARAMETERS_FOUND));
        }
    }

    private void HandleConstructorChecks()
    {
        int indexOfOpenBracket = bodyElements.IndexOf("(");
        List<string> body = bodyElements[..indexOfOpenBracket];

        IEnumerable<string> keywords = body.Where(Keywords.ALL_KEYWORDS.Contains);
        foreach (var word in keywords)
        {
            if (!Keywords.ACCESS_MODIFIERS.Contains(word))
            {
                // Not access modifier, not allowed.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetWrongContextMessage(word)));
            }
        }

        IEnumerable<string> correctModifiers = body.Where(x => Keywords.ACCESS_MODIFIERS.Contains(x));
        IEnumerable<string> duplicates = correctModifiers.GroupBy(x => x).Where(group => group.Count() != 1).Select(x => x.Key).Distinct();
        foreach (var duplicate in duplicates)
        {
            // There is some duplicates.
            exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetDuplicateWordMessage(duplicate)));
        }

        List<int> problematicKeywords = [];
        bool startAdding = false;
        for (int i = 0; i < body.Count; i++)
        {
            if (!Keywords.ALL_KEYWORDS.Contains(body[i]))
            {
                startAdding = true;
            }
            else if (startAdding)
            {
                problematicKeywords.Add(i);
            }
        }
        if (problematicKeywords.Count != 0)
        {
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.MODIFIER_ORDER));
        }

        int indexOfLastBracket = bodyElements.IndexOf(")");
        List<string> parameterContent = bodyElements[(indexOfOpenBracket + 1)..indexOfLastBracket];
        var paramParser = new ParametersParser(dimension.begin, parameterContent);
        paramParser.ParseBody();
        exceptions.AddRange(paramParser.Exceptions);
    }

    private void HandleMethodChecks()
    {
        int indexOfOpenBracket = bodyElements.IndexOf("(");
        List<string> body = bodyElements[..indexOfOpenBracket];

        IEnumerable<string> keywords = body.Where(Keywords.ALL_KEYWORDS.Contains);
        foreach (var word in keywords)
        {
            if (!Keywords.ACCESS_MODIFIERS.Contains(word))
            {
                // Not access modifier, not allowed.
                exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetWrongContextMessage(word)));
            }
        }

        IEnumerable<string> correctModifiers = body.Where(x => Keywords.ACCESS_MODIFIERS.Contains(x));
        IEnumerable<string> duplicates = correctModifiers.GroupBy(x => x).Where(group => group.Count() != 1).Select(x => x.Key).Distinct();
        foreach (var duplicate in duplicates)
        {
            // There is some duplicates.
            exceptions.Add(new ModifierException(dimension.begin, ModifierException.GetDuplicateWordMessage(duplicate)));
        }

        List<int> problematicKeywords = [];
        bool startAdding = false;
        for (int i = 0; i < body.Count; i++)
        {
            if (!Keywords.ALL_KEYWORDS.Contains(body[i]))
            {
                startAdding = true;
            }
            else if (startAdding)
            {
                problematicKeywords.Add(i);
            }
        }
        if (problematicKeywords.Count != 0)
        {
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.MODIFIER_ORDER));
        }

        int indexOfLastBracket = bodyElements.IndexOf(")");
        List<string> parameterContent = bodyElements[(indexOfOpenBracket + 1)..indexOfLastBracket];
        var paramParser = new ParametersParser(dimension.begin, parameterContent);
        paramParser.ParseBody();
        exceptions.AddRange(paramParser.Exceptions);
    }

    public GenericContext ParseBody()
    {
        bodyElements = IdentifierSplitTools.SplitBody(cleanBody);

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
            HandleInterfaceChecks();
            contextToReturn = new GenericContext(MemberType.Interface);
        }
        else if (bodyElements.Contains(Keywords.ENUM))
        {
            HandleEnumChecks();
            contextToReturn = new GenericContext(MemberType.Enum);
        }
        else if (bodyElements.Contains("(") && bodyElements.Contains(")"))
        {
            MemberType type = IdentifyMethodType();
            switch (type)
            {
                case MemberType.Destructor:
                    HandleDestructorChecks();
                    break;
                case MemberType.Constructor:
                    HandleConstructorChecks();
                    break;
                case MemberType.Method:
                    HandleMethodChecks();
                    break;
            }
            contextToReturn = new(type);
        }
        else
        {
            contextToReturn = new(MemberType.Unknown);
        }
        return contextToReturn;
    }
}