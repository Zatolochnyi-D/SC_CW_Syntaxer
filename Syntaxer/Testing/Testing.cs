namespace Syntaxer.Testing;

public class StringFormats
{
    public StringFormats()
    {
        // Those are examples of correctly used strings.
        // Unaccounted string formats are not included.
        // Commented out examples are incorrect.
        string defaultString1 = "It's a default string.";
        string defaultString2 = "This string must not include end of line symbol (\\n).";
        string defaultString3 = "It always starts and ends with double quotes, so if on one line detected only one double quote symbol, it's an error";
        string defaultString4 = "Backslash + double quote symbol (\\\") is not a double quote, it should be ommited.";
        string formatString = $"this string can have {"variables"} inside";
    }
}

// // One line comment
// int val = 9; // Still one line comment

/* Multiline comment in one line */
// /* one line comment in 
// many lines */
// int cal = /* Even this
// is possible */ 9;

// string str1 = "gegrege";
// string str2 = @" sdgdfgfgd
// gdfgdfgfg";
// string str = "dfgdfgdf \r\n \r \n \n\r sdgfdfgdf";
// string str3 = """fsdfddg "AAAA" """;
/* sdfergergjerng
fdsfsdfwef*/