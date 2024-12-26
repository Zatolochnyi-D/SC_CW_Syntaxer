using Syntaxer.Members;

namespace Syntaxer;

public class Program
{
    public static void Main(string[] args)
    {
        IEnumerable<string> files = Directory.EnumerateFiles("/Users/denis/Desktop/SC_things_testing", "*.cs", SearchOption.AllDirectories);
        files = files.Where(x => !x.Contains("/obj/Debug/")).Where(x => !x.Contains("/bin/Debug/"));
        IEnumerable<string> fileContents = files.Select(File.ReadAllText);
        List<ScriptFile> scripts = fileContents.Select(x => new ScriptFile(x)).ToList();
    }
}