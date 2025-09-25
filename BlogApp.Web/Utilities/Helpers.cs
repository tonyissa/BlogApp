namespace BlogApp.Web.Utilities;

public static class Helpers
{
    public static string Sluggify(string input) => input
        .ToLower()
        .Replace(' ', '-')
        .Where(c => char.IsLetterOrDigit(c) || c == '-')
        .ToString() ?? "";
}