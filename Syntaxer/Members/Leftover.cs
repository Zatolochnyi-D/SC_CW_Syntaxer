using System.Reflection.Metadata.Ecma335;
using Syntaxer.Context;
using Syntaxer.Enumerations;
using Syntaxer.Exceptions;
using Syntaxer.Operations;
using Syntaxer.Parsers;

namespace Syntaxer.Members;

/// <summary>
/// Used for everything that left at the end of block parsing.
/// </summary>
public class Leftover : IMember
{
    private ScriptFile parentFile;
    private IMember parent;
    private string body;
    private (int begin, int end) dimension;
    private GenericContext context;
    private List<SyntaxException> exceptions = [];

    public ScriptFile ParentFile => parentFile;
    public GenericContext Context => context;
    public IMember Parent => parent;
    public List<SyntaxException> Exceptions => exceptions;


    public Leftover(string leftoverContent, (int, int) dimension, IMember parent)
    {
        body = leftoverContent;
        this.dimension = dimension;
        this.parent = parent;
        parentFile = parent.ParentFile;
        context = new(MemberType.Unknown);
    }

    public void SplitContent()
    {
        if (parent.Context.MemberType == MemberType.Enum)
        {
            context = new GenericContext(MemberType.EnumContent);
            // This leftover is enumeration.
            List<string> bodyElements = IdentifierSplitTools.SplitBody(string.Join("", body.Where(x => x != '\n')));
            if (bodyElements[^1] == ",")
            {
                bodyElements = bodyElements[..^1];
            }
            Console.WriteLine();
            var parser = new EnumerationParser(bodyElements, dimension, [","]);
            List<Operation> operations = parser.ParseBody();
            if (operations.Count != 1)
            {
                // There is missed ",".
                exceptions.Add(new EnumDeclarationException(dimension.begin, EnumDeclarationException.DECLARATION_MISSING_MESSAGE));
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
        else
        {
            exceptions.Add(new LeftoverException(dimension.begin));
        }
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

    public List<SyntaxException> CollectExceptions()
    {
        return exceptions;
    }
}