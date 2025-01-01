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
        List<string> filePathes =
        [
            "/Users/denis/Desktop/SC_things_testing/Testing/Strings.cs",
            "/Users/denis/Desktop/SC_things_testing/Testing/Blocks.cs",
            "/Users/denis/Desktop/SC_things_testing/Testing/Namespaces.cs",
            "/Users/denis/Desktop/SC_things_testing/Testing/Classes.cs",
            "/Users/denis/Desktop/SC_things_testing/Testing/Enums.cs",
            // "/Users/denis/Desktop/SC_CW_Syntaxer/Syntaxer/Members/ScriptFile.cs"
        ];

        foreach (var path in filePathes)
        {
            Console.WriteLine($"In file {path}:");
            string data = File.ReadAllText(path);
            ScriptFile file = new(data);
            file.SplitContent();
            foreach (var exception in file.ExceptionsAsStrings())
            {
                Console.Write("  ");
                Console.WriteLine(exception);
            }
        }
    }
}