namespace Syntaxer.Operations;

public class AccessOperation
{
    private List<string> parts = [];
    private int leftEndIndex = 0;
    private int rightEndIndex = 0;

    public AccessOperation(params string[] parts)
    {
        this.parts.AddRange(parts);
        rightEndIndex = parts.Length - 1;
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
            return new AccessOperation(parts.Concat(b.parts.Skip(1)).ToArray());
        }
        else if (GetLeft() == b.GetRight())
        {
            return new AccessOperation(b.parts.Concat(parts.Skip(1)).ToArray());
        }
        else
        {
            return null;
        }
    }

    public override string ToString()
    {
        return string.Join(".", parts);
    }
}