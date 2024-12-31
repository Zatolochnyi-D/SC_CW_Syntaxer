using Syntaxer.Exceptions;
using Syntaxer.Validators;

namespace Syntaxer.Operations;

public class AccessOperation
{
    private List<string> parts = [];
    private int leftEndIndex = 0;
    private int rightEndIndex = 0;
    private int position;
    private List<SyntaxException> exceptions = [];

    public List<SyntaxException> Exceptions => exceptions;

    public AccessOperation(int position, params string[] parts)
    {
        this.parts.AddRange(parts);
        rightEndIndex = parts.Length - 1;
        this.position = position;
    }

    public string GetLeft()
    {
        return parts[leftEndIndex];
    }

    public string GetRight()
    {
        return parts[rightEndIndex];
    }

    public AccessOperation? TryMerge(AccessOperation b)
    {
        if (GetRight() == b.GetLeft())
        {
            return new AccessOperation(position, parts.Concat(b.parts.Skip(1)).ToArray());
        }
        else if (GetLeft() == b.GetRight())
        {
            return new AccessOperation(b.position, b.parts.Concat(parts.Skip(1)).ToArray());
        }
        else
        {
            return null;
        }
    }

    public void Validate()
    {
        if (parts.Contains(""))
        {
            // There is improperly located dots.
            exceptions.Add(new LongWordException(position, LongWordException.WORD_INCOMPLETE_MESSAGE));
        }
        var validator = new NameValidator(position, [.. parts]);
        validator.Validate();
        exceptions.AddRange(validator.Exceptions);
    }

    public override string ToString()
    {
        return string.Join(".", parts);
    }
}