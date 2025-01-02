
using System.Security.Cryptography;
using Syntaxer.Context;
using Syntaxer.Enumerations;
using Syntaxer.Exceptions;
using Syntaxer.Operations;
using Syntaxer.Parsers;
using Syntaxer.Validators;

namespace Syntaxer.Members;

/// <summary>
/// Used to store any code that ends with ";"
/// </summary>
public class Instruction : IMember
{
    private ScriptFile parentFile;
    private IMember parent;
    private string body;
    private List<string> bodyElements = [];
    private (int begin, int end) dimension;
    private List<SyntaxException> exceptions = [];
    private GenericContext context = new(MemberType.Unknown);

    public ScriptFile ParentFile => parentFile;
    public IMember Parent => parent;
    public List<SyntaxException> Exceptions => exceptions;
    public GenericContext Context => context;

    public Instruction(string instructionContent, (int, int) dimension, IMember parent)
    {
        body = instructionContent[..^1];
        dimension.Item2--;
        this.parent = parent;
        parentFile = parent.ParentFile;
        // Begin of given dimension is first character after end of previous instruction/block. This value need refining.
        // End is ";", as otherwise no instruction could be created at first place. This value can be taken as is.
        this.dimension = dimension;

        // Iterate through first characters in body to find first non-empty character. It's position should be taken as beginning.
        for (int i = 0; i < instructionContent.Length; i++)
        {
            if (!char.IsWhiteSpace(instructionContent[i]))
            {
                // It's first non-empty character at the body's beginning. It should be begin position.
                this.dimension.begin += i;
                break;
            }
        }
    }

    private void HandleUsingChecks()
    {
        if (bodyElements.IndexOf(Keywords.USING) != 0)
        {
            // Keyword is not first.
            exceptions.Add(new UsingException(dimension.begin, UsingException.KEYWORD_IS_NOT_FIRST_MESSAGE));
            return;
        }
        MemberType locationType = parent.Parent.Context.MemberType;
        if (locationType != MemberType.File)
        {
            // Placed in the wrong location.
            exceptions.Add(new UsingException(dimension.begin, UsingException.INCORRECT_PLACE_MESSAGE));
            return;
        }
        else
        {
            if (!parentFile.IsOnTop(this))
            {
                // Placed not on top.
                exceptions.Add(new UsingException(dimension.begin, UsingException.INCORRECT_PLACE_MESSAGE));
                return;
            }
        }

        // At this point, there is only one using word, and it is first word in the sequece.
        List<string> leftover = bodyElements[1..];
        if (leftover.Count == 0)
        {
            // There is no name provided.
            exceptions.Add(new UsingException(dimension.begin, UsingException.NO_NAME_DECLARED_MESSAGE));
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
            exceptions.Add(new UsingException(dimension.begin, UsingException.MANY_NAMES_DECLARED_MESSAGE));
            return;
        }
    }

    private void HandleDelegateChecks()
    {
        MemberType locationType = parent.Parent.Context.MemberType;
        MemberType[] allowedTypes = [MemberType.File, MemberType.Namespace, MemberType.Class, MemberType.Interface];
        if (!allowedTypes.Contains(locationType))
        {
            // Placed in the wrong location.
            exceptions.Add(new TypePlaceException(dimension.begin));
        }

        int indexOfClassKeyword = bodyElements.IndexOf(Keywords.DELEGATE);
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
        List<string> parameters = [];
        if (indexOfClassKeyword + 1 != bodyElements.Count)
        {
            // There is something on right.
            List<string> rightPart;
            rightPart = bodyElements[(indexOfClassKeyword + 1)..];
            int openBracket = rightPart.IndexOf("(");
            int closeBracket = rightPart.IndexOf(")");
            if (openBracket == -1 || closeBracket == -1)
            {
                // Parameters brackets specified incorrectly.
                exceptions.Add(new DelegateException(dimension.begin, DelegateException.BRACKETS_ERROR));
                name = rightPart;
            }
            else
            {
                name = rightPart[..openBracket];
                parameters = rightPart[(openBracket + 1)..closeBracket];
            }
        }

        var nameValidator = new NameValidator(dimension.begin, [.. name]);
        nameValidator.Validate();
        exceptions.AddRange(nameValidator.Exceptions);

        if (name.Count == 0)
        {
            // Name is empty.
            exceptions.Add(new DelegateException(dimension.begin, DelegateException.NO_NAME_PROVIDED));
        }
        else if (name.Count == 1)
        {
            exceptions.Add(new DelegateException(dimension.begin, DelegateException.TYPE_NOT_FOUND));
        }
        else if (name.Count > 2)
        {
            // To many words.
            exceptions.Add(new DelegateException(dimension.begin, DelegateException.MANY_NAMES_PROVIDED));
        }

        Console.WriteLine(parameters.Count);
        foreach ( var el in parameters)
        {
            Console.WriteLine(el);
        }
        Console.WriteLine();
        if (parameters.Count != 0)
        {
            var paramsParser = new ParametersParser(dimension.begin, parameters);
            paramsParser.ParseBody();
            exceptions.AddRange(paramsParser.Exceptions);
        }
    }

    private MemberType IdentifyMember()
    {
        if (parent.Context.MemberType == MemberType.Interface)
        {
            return MemberType.MethodSignature;
        }
        return MemberType.Unknown;
    }

    private void HandleSignatureChecks()
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
        int lastModifierIndex = -1;
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
            
            if (Keywords.ACCESS_MODIFIERS.Contains(body[i]))
            {
                lastModifierIndex = i;
            }
        }
        
        if (problematicKeywords.Count != 0)
        {
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.MODIFIER_ORDER));
        }

        
        List<string> words;
        if (lastModifierIndex == -1)
        {
            words = body[..indexOfOpenBracket];
        }
        else
        {
            words = body[(lastModifierIndex + 1)..indexOfOpenBracket];
        }
        if (words.Count == 0)
        {
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.NO_NAME_FOUND));
        }
        else if (words.Count == 1)
        {
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.NOT_ENOUGH_WORDS));
        }
        else if (words.Count > 2)
        {
            exceptions.Add(new MethodDeclarationException(dimension.begin, MethodDeclarationException.TO_MANY_WORDS));
        }

        int indexOfLastBracket = bodyElements.IndexOf(")");
        List<string> parameterContent = bodyElements[(indexOfOpenBracket + 1)..indexOfLastBracket];
        var paramParser = new ParametersParser(dimension.begin, parameterContent);
        paramParser.ParseBody();
        exceptions.AddRange(paramParser.Exceptions);
    }

    public void SplitContent()
    {
        bodyElements = IdentifierSplitTools.SplitBody(new string(body.Where(x => !char.IsControl(x)).ToArray()));
        if (bodyElements.Contains(Keywords.USING))
        {
            HandleUsingChecks();
            context = new GenericContext(MemberType.Using);
        }
        else if (bodyElements.Contains(Keywords.DELEGATE))
        {
            HandleDelegateChecks();
            context = new GenericContext(MemberType.Delegate);
        }
        else if (bodyElements.Contains("(") && bodyElements.Contains(")"))
        {
            MemberType type = IdentifyMember();
            if (type == MemberType.MethodSignature)
            {
                HandleSignatureChecks();
            }
            context = new GenericContext(type);
        }
        if (context.MemberType == MemberType.Unknown)
        {
            exceptions.Add(new UnknownInstructionException(dimension.begin));
        }
    }

    public List<SyntaxException> CollectExceptions()
    {
        return exceptions;
    }

    public override string ToString()
    {
        string result = "";
        (int line, int column) start = parentFile.IndexToCoordinates(dimension.begin);
        (int line, int column) end = parentFile.IndexToCoordinates(dimension.end);
        result += $"s: {dimension.begin}, e: {dimension.end}; l: {start.line}, c: {start.column}; l: {end.line}, c: {end.column} -> ";
        result += body.Replace('\n', '\0').Trim();
        return result;
    }
}