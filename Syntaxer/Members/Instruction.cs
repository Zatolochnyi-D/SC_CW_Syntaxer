
using Syntaxer.Context;
using Syntaxer.Enumerations;
using Syntaxer.Exceptions;
using Syntaxer.Operations;
using Syntaxer.Parsers;

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
    private GenericContext context = new(Enumerations.MemberType.Unknown);

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
            Console.WriteLine("DELEGATE");
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