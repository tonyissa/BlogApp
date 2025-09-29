using BlogApp.Web.Data.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Tests.Helpers;

public static class TokenHelper
{
    public static string GenerateToken(CommentDTO comment)
    {
        var input = $"{comment.Name}{comment.Text}{comment.DatePosted.Ticks}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes).Replace("/", "_").Replace("+", "-");
    }
}