namespace Syntax;

public class Program
{
    public static void Main(string[] args)
    {
        string data = File.ReadAllText("../../../Second.cs");
        Console.WriteLine(data);
    }
}