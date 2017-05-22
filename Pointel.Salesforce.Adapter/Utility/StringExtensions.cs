namespace Pointel.Salesforce.Adapter.Utility
{
    public static class StringExtensions
    {
        public static string RemoveSpecialCharacters(this string str)
        {
            str = str.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
            return str;// Regex.Replace(str, @"[^\s]", "");
        }
    }
}