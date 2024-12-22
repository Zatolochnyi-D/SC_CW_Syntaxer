namespace Syntax;

public class Program
{
    public static void Main(string[] args)
    {
        string data = File.ReadAllText("../../../Target.cs");
        ScriptFile file = new(data);
    }
}