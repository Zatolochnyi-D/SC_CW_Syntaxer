namespace Syntaxer.Literals;

public class StringLiteral
{
    private string body;
    private (int begin, int end) dimension;

    public StringLiteral(string stringContent, (int, int) dimension)
    {
        body = stringContent;
        this.dimension = dimension;
    }
}