namespace Syntaxer;

public static class Keywords
{
    #region Keywords
    public const string NAMESPACE = "namespace";

    public const string CLASS = "class";
    public const string INTERFACE = "interface";
    public const string ENUM = "enum";
    public const string DELEGATE = "delegate";

    public const string PRIVATE = "private";
    public const string PROTECTED = "protected";
    public const string PUBLIC = "public";
    public const string INTERNAL = "internal";

    public const string STATIC = "static";
    public const string ABSTRACT = "abstract";
    public const string SEALED = "sealed";

    public const string VIRTUAL = "virtual";
    public const string BASE = "base";
    public const string BREAK = "break";
    public const string CONST = "const";
    public const string CONTINUE = "continue";
    public const string IF = "if";
    public const string ELSE = "else";
    public const string FOR = "for";
    public const string NEW = "new";
    public const string OUT = "out";
    public const string REF = "ref";
    public const string PARAMS = "params";
    public const string READONLY = "readonly";
    public const string RETURN = "return";
    public const string THIS = "this";
    public const string USING = "using";

    public static readonly string[] TYPES =
    [
        CLASS,
        INTERFACE,
        ENUM,
        DELEGATE,
    ];

    public static readonly string[] ACCESS_MODIFIERS =
    [
        PRIVATE,
        PROTECTED,
        PUBLIC,
        INTERNAL
    ];

    public static readonly string[] CLASS_MODIFIERS =
    [
        .. ACCESS_MODIFIERS,
        STATIC,
        ABSTRACT,
        SEALED,
    ];
    public static readonly string[] ALL_KEYWORDS =
    [
        .. CLASS_MODIFIERS,
        .. TYPES,
        NAMESPACE,
        VIRTUAL,
        BASE,
        BREAK,
        CONST,
        CONTINUE,
        IF,
        ELSE,
        FOR,
        NEW,
        OUT,
        REF,
        PARAMS,
        READONLY,
        RETURN,
        THIS,
        USING,
    ];
    #endregion

    #region Operators
    public const string INHERITANCE_OPERATOR = ":";

    public const string ACCESS_OPERATOR = ".";
    public const string NULL_ACCESS_OPERATOR = "?.";

    public const string ADD_OPERATOR = "+";
    public const string SUBTRACT_OPERATOR = "-";
    public const string MULTIPLY_OPERATOR = "*";
    public const string DIVIDE_OPERATOR = "/";
    public const string REMINDER_OPERATOR = "%";
    public const string INCREMENT_OPERATOR = "++";
    public const string DECREMENT_OPERATOR = "--";

    public const string AND_OPERATOR = "&&";
    public const string OR_OPERATOR = "||";
    public const string EQUAL_OPERATOR = "==";
    public const string NOT_EQUAL_OPERATOR = "!=";
    public const string LESS_OPERATOR = "<";
    public const string GREATER_OPERATOR = ">";
    public const string LESS_EQUAL_OPERATOR = "<=";
    public const string GREATER_EQUAL_OPERATOR = ">=";

    public const string ASSIGN_OPERATOR = "=";
    public const string ADD_ASSIGN_OPERATOR = "+=";
    public const string SUBTRACT_ASSIGN_OPERATOR = "-=";
    public const string MULTIPLY_ASSIGN_OPERATOR = "*=";
    public const string DIVIDE_ASSIGN_OPERATOR = "/=";
    public const string REMINDER_ASSIGN_OPERATOR = "%=";

    public static readonly string[] ACCESS_OPERATORS =
    [
        ACCESS_OPERATOR,
        NULL_ACCESS_OPERATOR,
    ];

    public static readonly string[] MATH_OPERATORS =
    [
        ADD_OPERATOR,
        SUBTRACT_OPERATOR,
        MULTIPLY_OPERATOR,
        DIVIDE_OPERATOR,
        REMINDER_OPERATOR,
        INCREMENT_OPERATOR,
        DECREMENT_OPERATOR,
    ];

    public static readonly string[] LOGICAL_OPERATORS =
    [
        AND_OPERATOR,
        OR_OPERATOR,
        EQUAL_OPERATOR,
        NOT_EQUAL_OPERATOR,
        LESS_OPERATOR,
        GREATER_OPERATOR,
        LESS_EQUAL_OPERATOR,
        GREATER_EQUAL_OPERATOR,
    ];

    public static readonly string[] ASSIGN_OPERATORS =
    [
        ASSIGN_OPERATOR,
        ADD_ASSIGN_OPERATOR,
        SUBTRACT_ASSIGN_OPERATOR,
        MULTIPLY_ASSIGN_OPERATOR,
        DIVIDE_ASSIGN_OPERATOR,
        REMINDER_ASSIGN_OPERATOR,
    ];

    public static readonly string[] NON_ACCESS_OPERATORS =
    [
        INHERITANCE_OPERATOR,
        .. MATH_OPERATORS,
        .. LOGICAL_OPERATORS,
        .. ASSIGN_OPERATORS,
    ];

    public static readonly string[] ALL_OPERATORS =
    [
        .. NON_ACCESS_OPERATORS,
        .. ACCESS_OPERATORS,
    ];
    #endregion
}