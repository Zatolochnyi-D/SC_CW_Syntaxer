using Syntaxer.Enumerations;

namespace Syntaxer.Context;

public class GenericContext
{
    private MemberType memberType;

    public MemberType MemberType => memberType;

    public GenericContext(MemberType memberType)
    {
        this.memberType = memberType;
    }
}