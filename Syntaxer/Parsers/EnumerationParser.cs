using Syntaxer.Operations;

namespace Syntaxer.Parsers;

/// <summary>
/// Takes sequence of what supposed to be a long name, returns list of names it found.
/// </summary>
public class EnumerationParser
{
    private List<string> body;
    private (int begin, int end) dimension;

    public EnumerationParser(List<string> expectedEnumeration, (int, int) dimension)
    {
        body = expectedEnumeration;
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
                else
                {
                    left = body[leftIndex];
                }

                int rightIndex = i + 1;
                string right;
                if (rightIndex == body.Count || body[rightIndex] == ".")
                {
                    // There is nothing on right or there is other dot.
                    right = "";
                }
                else
                {
                    right = body[rightIndex];
                }
                operations.Enqueue(new AccessOperation(dimension.begin, left, right));
            }
        }

        List<AccessOperation> mergedOperations = [];
        // When body consists of separate words, without dots, queue will be empty.
        if (operations.Count != 0)
        {
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
        }

        List<int> usedIndexes = [];
        for (int i = 0; i < body.Count; i++)
        {
            if (body[i] == ".")
            {
                usedIndexes.Add(i - 1);
                usedIndexes.Add(i);
                usedIndexes.Add(i + 1);
            }
        }
        for (int i = 0; i < body.Count; i++)
        {
            if (!usedIndexes.Contains(i))
            {
                mergedOperations.Add(new(dimension.begin, body[i]));
            }
        }

        return mergedOperations;
    }
}