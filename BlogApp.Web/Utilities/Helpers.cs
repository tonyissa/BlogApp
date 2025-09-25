using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Web.Utilities;

public static class Helpers
{
    public static string Sluggify(string input) => input
        .ToLower()
        .Replace(' ', '-')
        .Where(c => char.IsLetterOrDigit(c) || c == '-')
        .ToString() ?? "";

    public static string GenerateToken(string authorName, string content, DateTime createdAt)
    {
        var input = $"{authorName}{content}{createdAt.Ticks}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-");
    }
}