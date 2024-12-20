namespace Syntax;

public class Program
{
    public static void Main(string[] args)
    {
        string data = File.ReadAllText("../../../Second.cs");
        // Console.WriteLine(data);
        // Console.WriteLine(data.Count(x => x == '\n'));
        int column = 0;
        int line = 0;
        List<(int x, int y)> sizes = new();
        foreach (var ch in data)
        {
            if (ch == '\n')
            {
                sizes.Add((column, line));
                line++;
                column = 0;
                continue;
            }
            column++;
        }
        sizes.Add((column, line));
        foreach (var pair in sizes)
        {
            Console.WriteLine($"Length: {pair.x}, Line: {pair.y}");
        }
    }
}