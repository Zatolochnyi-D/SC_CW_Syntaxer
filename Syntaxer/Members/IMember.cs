namespace Syntaxer.Members;

public interface IMember
{
    public ScriptFile ParentFile { get; }

    public void SplitContent();
}