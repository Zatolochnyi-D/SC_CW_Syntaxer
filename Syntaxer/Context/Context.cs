namespace Syntaxer.Context;

public enum TopLevelMemberType
{
    Namespace,
    Class,
    Struct,
    Interface,
    Enum
}

public class Context
{
    private TopLevelMemberType memberType;

    public TopLevelMemberType MemberType => memberType;

    public Context(TopLevelMemberType memberType)
    {
        this.memberType = memberType;
    }
}