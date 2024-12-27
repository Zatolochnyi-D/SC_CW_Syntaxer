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

public class GenericContext
{
    private MemberType memberType;

    public MemberType MemberType => memberType;

    public GenericContext(MemberType memberType)
    {
        this.memberType = memberType;
    }
}