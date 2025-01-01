namespace Syntaxer;

public static class IdentifierSplitTools
{
    public static List<string> SplitStringBySymbols(string target)
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
                    if (Keywords.ALL_KEYWORDS.Contains(possibleOperator))
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

    public static List<string> SplitBody(string cleanBody)
    {
        IEnumerable<string> splitResult = cleanBody.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        List<string> result = [];
        foreach (var element in splitResult)
        {
            result.AddRange(SplitStringBySymbols(element));
        }
        return result;
    }
}