using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Tests.Integration;

public class MockedAntiforgery : IAntiforgery
{
    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        => new("test", "test", "form", "cookie");

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
        => new("test", "test", "form", "cookie");

    public Task<bool> IsRequestValidAsync(HttpContext httpContext)
        => Task.FromResult(true);

    public void SetCookieTokenAndHeader(HttpContext httpContext) { }

    public Task ValidateRequestAsync(HttpContext httpContext)
        => Task.CompletedTask;
}