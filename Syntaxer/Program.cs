namespace Syntax;

public class Program
{
    public static void Main(string[] args)
    {
        string data = File.ReadAllText("../../../Second.cs");
        ScriptFile file = new(data);
        file.ClearBody();
    }
}