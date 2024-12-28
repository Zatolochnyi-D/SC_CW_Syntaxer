using Syntaxer.Members;
using Syntaxer.Parsers;

namespace Syntaxer.Validators;

/// <summary>
/// Class responsible for identifying erroneous strings if file.
/// Collects correct strings and errors for incorrect strings.
/// </summary>
public class StringValidator : Parser
{
    private ScriptFile parent;
    private (int begin, int end) dimension;

    public StringValidator(string fileContent, (int, int) fileDimension, ScriptFile parent) : base(fileContent)
    {
        this.parent = parent;
        dimension = fileDimension;
    }

    public void Validate()
    {
        
    }
}