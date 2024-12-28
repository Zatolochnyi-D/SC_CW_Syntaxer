using Syntaxer.Exceptions;

namespace Syntaxer.Members;

public interface IMember
{
    public ScriptFile ParentFile { get; }
    public List<SyntaxException> Exceptions { get; }

    public void SplitContent();
}