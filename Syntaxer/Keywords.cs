namespace Syntaxer;

public static class Keywords
{
    public const string NAMESPACE = "namespace";

    public const string CLASS = "class";
    public const string INTERFACE = "interface";
    public const string ENUM = "enum";

    public static readonly string[] TYPES =
    [
        CLASS,
        INTERFACE,
        ENUM,
    ];

    public const string PRIVATE = "private";
    public const string PROTECTED = "protected";
    public const string PUBLIC = "public";
    public const string INTERNAL = "internal";

    public static readonly string[] ACCESS_MODIFIERS =
    [
        PRIVATE,
        PROTECTED,
        PUBLIC,
        INTERNAL
    ];

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
    ];

    public static readonly string[] ALL_OPERATORS =
    [
        .. OPERATORS,
        .. ACCESS_OPERATORS,
    ];

    public static readonly string[] ALL_KEYWORDS =
    [
        .. ACCESS_MODIFIERS,
        .. TYPES,
        NAMESPACE,
    ];
}