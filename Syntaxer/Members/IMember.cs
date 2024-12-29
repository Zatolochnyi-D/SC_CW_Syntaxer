using Syntaxer.Context;
using Syntaxer.Exceptions;

namespace Syntaxer.Members;

public interface IMember
{
    public ScriptFile ParentFile { get; }
    public IMember Parent { get; }
    public GenericContext Context { get; }
    public List<SyntaxException> Exceptions { get; }

    public void SplitContent();
    public List<SyntaxException> CollectExceptions();
}