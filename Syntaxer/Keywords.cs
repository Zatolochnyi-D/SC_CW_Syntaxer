namespace Syntaxer;

public static class Keywords
{
    public const string NAMESPACE = "namespace";

    public static readonly string[] ACCESS_OPERATORS =
    [
        ".",
        "?.",
    ];

    public static readonly string[] OPERATORS =
    [
        "+",
        "-",
        "*",
        "/",
        "%",
        "&",
        "&&",
        "|",
        "||",
        "==",
        "!=",
        "<",
        ">",
        "<=",
        ">=",
        "=",
        "+=",
        "-=",
        "*=",
        "/=",
        "%=",
        "++",
        "--",
        "=>",
        .. ACCESS_OPERATORS
    ];
}