using Syntaxer.Exceptions;

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

    public List<string>? ParseBody()
    {
        // Gather indexes of words and dots.
        List<int> names = [];
        List<int> dots = [];
        for (int i = 0; i < body.Count; i++)
        {
            if (body[i] == ".") dots.Add(i);
            else names.Add(i);
        }

        foreach (var position in dots)
        {
            if (dots.Contains(position + 1))
            {
                // There are sequence of dots.
                // Throw critical error.
                exceptions.Add(new LongWordException(dimension.begin, LongWordException.WORD_INCOMPLETE_MESSAGE));
                return null;
            }
        }

        // Gather indexes of neighbours of dots.
        List<int> dotNeighbours = [];
        foreach (var position in dots)
        {
            dotNeighbours.Add(position - 1);
            dotNeighbours.Add(position + 1);
        }

        if (dotNeighbours.Contains(-1) || dotNeighbours.Contains(body.Count))
        {
            // One of dots don't have beginning or ending word. 
            // Throw critical error.
            exceptions.Add(new LongWordException(dimension.begin, LongWordException.WORD_INCOMPLETE_MESSAGE));
            return null;
        }

        // Go through selected indexes and count amount of long words.
        // Remove indexes used in traversing from names indexes list to form list of single (short) words.
        int wordCount = 0;
        List<List<int>> words = [[]];
        bool firstInSequence = true;
        for (int i = 0; i < dotNeighbours.Count; i++)
        {
            words[wordCount].Add(dotNeighbours[i]);
            names.Remove(dotNeighbours[i]);
            if (firstInSequence)
            {
                firstInSequence = false;
                continue;
            }
            if (i == dotNeighbours.Count - 1) continue;
            if (dotNeighbours[i] == dotNeighbours[i + 1])
            {
                i++;
            }
            else
            {
                firstInSequence = true;
                words.Add([]);
                wordCount++;
            }
        }

        // Add short words separately.
        foreach (var position in names)
        {
            words.Add([]);
            wordCount++;
            words[wordCount].Add(position);
        }

        // Join words into solid strings.
        List<string> results = [];
        foreach (var positions in words)
        {
            results.Add(string.Join(".", positions.Select(x => body[x])));
        }

        return results;
    }
}