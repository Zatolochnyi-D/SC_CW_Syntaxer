using Syntaxer.Exceptions;

namespace Syntaxer.Parsers;

public class ParametersParser
{
    private List<string> body;
    private List<List<string>> parameters = [];
    private int start;
    private List<SyntaxException> exceptions = [];

    public List<SyntaxException> Exceptions => exceptions;

    public ParametersParser(int start, List<string> content)
    {
        this.start = start;
        body = new(content);
    }

    private void SplitBody()
    {
        if (body.Count == 0) return;
        List<string> parameter = [];
        foreach (var el in body)
        {
            if (el == ",")
            {
                parameters.Add(parameter);
                parameter = [];
                continue;
            }
            parameter.Add(el);
        }
        parameters.Add(parameter);
    }

    private void HandleSimpleDeclaration(List<string> parameter)
    {
        if (parameter.Count < 2)
        {
            exceptions.Add(new ParameterException(start, ParameterException.NOT_ENOUGH_WORDS));
        }
        else if (parameter.Count > 2)
        {
            exceptions.Add(new ParameterException(start, ParameterException.TO_MANY_NAMES));
        }
    }

    private void HandleAdvancedDeclaration(List<string> parameter)
    {
        int refIndex = parameter.IndexOf(Keywords.REF);
        int outIndex = parameter.IndexOf(Keywords.OUT);
        if (outIndex != 0 && refIndex != 0)
        {
            exceptions.Add(new ParameterException(start, ParameterException.REF_OUT_NOT_FIRST));
        }
        if (parameter.Count < 3)
        {
            exceptions.Add(new ParameterException(start, ParameterException.NOT_ENOUGH_WORDS));
        }
        else if (parameter.Count > 3)
        {
            exceptions.Add(new ParameterException(start, ParameterException.TO_MANY_NAMES));
        }
    }

    public void ParseBody()
    {
        SplitBody();
        if (parameters.Count == 0) return;
        if (parameters.Any(x => x.Count == 0))
        {
            exceptions.Add(new ParameterException(start, ParameterException.MISSING_PARAMETER));
        }

        bool dropScan = false;
        foreach (var parameter in parameters)
        {
            if (Keywords.ALL_OPERATORS.Any(parameter.Contains))
            {
                // There is an operator.
                exceptions.Add(new ParameterException(start, ParameterException.OPERATOR_FOUND));
                dropScan = true;
            }
            if (Keywords.ALL_KEYWORDS.Except(Keywords.PARAMETER_KEYWORDS).Any(parameter.Contains))
            {
                // There are prohibited keywords found.
                exceptions.Add(new ParameterException(start, ParameterException.KEYWORDS_FOUND));
                dropScan = true;
            }
        }
        if (dropScan) return;

        foreach (var parameter in parameters)
        {
            if (Keywords.PARAMETER_KEYWORDS.Any(parameter.Contains))
            {
                HandleAdvancedDeclaration(parameter);
            }
            else
            {
                HandleSimpleDeclaration(parameter);
            }
        }
    }
}