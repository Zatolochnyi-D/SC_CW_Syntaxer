using Syntaxer.Exceptions;
using Syntaxer.Operations;

namespace Syntaxer.Parsers;

/// <summary>
/// Takes sequence of what supposed to be a long name, returns list of names it found.
/// </summary>
public class LongNameParser
{
    private List<string> body;
    private (int begin, int end) dimension;
    private List<SyntaxException> exceptions = [];

    public List<SyntaxException> Exceptions => exceptions;

    public LongNameParser(List<string> expectedName, (int, int) dimension)
    {
        body = expectedName;
        this.dimension = dimension;
    }

    public List<AccessOperation> ParseBody()
    {
        Queue<AccessOperation> operations = [];
        for (int i = 0; i < body.Count; i++)
        {
            if (body[i] == ".")
            {
                int leftIndex = i - 1;
                string left;
                if (leftIndex == -1 || body[leftIndex] == ".")
                {
                    // There is nothing on left or there is other dot, take it as empty.
                    left = "";
                }
                else left = body[leftIndex];

                int rightIndex = i + 1;
                string right;
                if (rightIndex == body.Count || body[rightIndex] == ".")
                {
                    // There is nothing on right or there is other dot.
                    right = "";
                }
                else right = body[rightIndex];
                operations.Enqueue(new AccessOperation(left, right));
            }
        }

        List<AccessOperation> mergedOperations = [];
        AccessOperation currentOperation = operations.Dequeue();
        while (true)
        {
            for (int i = 0; i < operations.Count; i++)
            {
                AccessOperation nextOperation = operations.Dequeue();
                var result = currentOperation.TryMerge(nextOperation);
                if (result == null)
                {
                    mergedOperations.Add(currentOperation);
                    currentOperation = nextOperation;
                    break;
                }
                else currentOperation = result;
            }
            if (operations.Count == 0)
            {
                mergedOperations.Add(currentOperation);
                break;
            }
        }

        return mergedOperations;
    }
}