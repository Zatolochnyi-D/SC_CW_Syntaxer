namespace Syntaxer.Context;

public enum MemberType
{
    Unknown,
    File,
    Namespace,
    Class,
    Struct,
    Interface,
    Delegate,
    Enum
}

public class Context
{
    private MemberType memberType;

    public MemberType MemberType => memberType;

    public Context(MemberType memberType)
    {
        this.memberType = memberType;
    }
}