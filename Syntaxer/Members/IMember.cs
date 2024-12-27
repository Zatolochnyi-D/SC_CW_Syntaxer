using Syntaxer.Context;

namespace Syntaxer.Members;

public interface IMember
{
    public ScriptFile ParentFile { get; }

    public GenericContext Context { get; }

    public void SplitContent();
}