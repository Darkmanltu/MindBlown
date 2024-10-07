using System.Text.RegularExpressions;
public static class StringExtensions
{
    public static string CapitalizeWords(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        
        return Regex.Replace(str, @"\b[a-z]", m => m.Value.ToUpper());
    }
}
