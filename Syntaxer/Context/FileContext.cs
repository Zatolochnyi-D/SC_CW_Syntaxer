namespace Syntaxer.Context;

public class FileContext : Context
{
    private bool hasFileScopedNamespace = false;

    public bool HasFileScopedNamespace { get => hasFileScopedNamespace; set => hasFileScopedNamespace = value; }

    public FileContext(MemberType memberType) : base(memberType)
    {
    }
}