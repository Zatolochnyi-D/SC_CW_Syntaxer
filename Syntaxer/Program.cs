using Syntaxer.Members;

namespace Syntaxer;

public class Program
{
    public static void Main(string[] args)
    {
        // IEnumerable<string> files = Directory.EnumerateFiles("/Users/denis/Desktop/SC_things_testing", "*.cs", SearchOption.AllDirectories);
        // files = files.Where(x => !x.Contains("/obj/Debug/")).Where(x => !x.Contains("/bin/Debug/"));
        // IEnumerable<string> fileContents = files.Select(File.ReadAllText);
        // List<ScriptFile> scripts = fileContents.Select(x => new ScriptFile(x)).ToList();
        string data = File.ReadAllText("/Users/denis/Desktop/SC_CW_Syntaxer/Syntaxer/Members/ScriptFile.cs");
        ScriptFile file = new(data);
        file.SplitContent();
        string data2 = File.ReadAllText("/Users/denis/Desktop/SC_things_testing/Testing/Strings.cs");
        ScriptFile file2 = new(data2);
        file2.SplitContent();
    }
}