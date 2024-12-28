namespace Syntaxer.Literals;

public class StringLiteral
{
    private string body; // Due to nature of StringParser, this string will always start and end with " or '.
    private (int begin, int end) dimension;

    public StringLiteral(string stringContent, (int, int) dimension)
    {
        body = stringContent;
        this.dimension = dimension;
    }
}