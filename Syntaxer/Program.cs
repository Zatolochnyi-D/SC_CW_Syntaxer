using Syntaxer.Members;

namespace Syntaxer;

public class Program
{
    public static void Main(string[] args)
    {
        string data = File.ReadAllText("/Users/denis/Desktop/SC_CW_Syntaxer/Syntaxer/Members/ScriptFile.cs");
        ScriptFile file = new(data);
    }
}