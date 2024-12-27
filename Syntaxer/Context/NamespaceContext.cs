namespace Syntaxer.Context;

public class NamespaceContext : GenericContext
{
    private bool isInstruction = false;

    public bool IsInstruction { get => isInstruction; set => isInstruction = value; }

    public NamespaceContext(MemberType memberType) : base(memberType)
    {
    }
}