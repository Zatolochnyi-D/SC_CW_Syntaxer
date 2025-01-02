using Syntaxer.Members;

namespace Syntaxer;

public class Program
{
    public static void Main(string[] args)
    {
        string path;
        bool useRecursion;
        if (args.Length == 0)
        {
            Console.WriteLine("No parameters provided.");
            return;
        }
        else if (args.Length == 1)
        {
            // Only path provided.
            path = args[0];
            useRecursion = false;
        }
        else if (args.Length == 2)
        {
            path = args[1];
            if (args[0] == "-r")
            {
                useRecursion = true;
            }
            else
            {
                Console.WriteLine($"Unknown parameter {args[0]}.");
                return;
            }
        }
        else
        {
            Console.WriteLine("To many parameters provided. Enter correct amount of parameters.");
            return;
        }

        FileAttributes attributes;
        try
        {
            attributes = File.GetAttributes(path);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Target was not found. Ensure you entered correct path.");
            return;
        }

        if (attributes.HasFlag(FileAttributes.Directory))
        {
            // It's an directory.
            SearchOption searchOption = useRecursion ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            IEnumerable<string> files = Directory.EnumerateFiles(path, "*.cs", searchOption);
            IEnumerable<string> fileContents = files.Select(File.ReadAllText);
            foreach (var file in files.Zip(fileContents))
            {
                ScanFile(file.First, file.Second);
            }
        }
        else
        {
            ScanFile(path, File.ReadAllText(path));
        }
    }

    private static void ScanFile(string filePath, string fileContent)
    {
        Console.WriteLine($"In file {filePath}:");
        ScriptFile file = new(fileContent);
        file.SplitContent();
        foreach (var exception in file.ExceptionsAsStrings())
        {
            Console.Write("  ");
            Console.WriteLine(exception);
        }
    }
}