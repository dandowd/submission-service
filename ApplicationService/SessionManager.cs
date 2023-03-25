using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ApplicationService;

public class SessionManager : ISessionManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Create()
    {
        var sessionId = Guid.NewGuid().ToString();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, sessionId),
            new Claim(ClaimTypes.Role, "Applicant"),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        var principal = new ClaimsPrincipal(identity);

        if (_httpContextAccessor.HttpContext == null)
        {
            throw new Exception("HttpContext is null");
        }

        await _httpContextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return sessionId;
    }

    public string GetUserId()
    {
        var sessionId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        if (sessionId == null)
        {
            throw new Exception("User is not authenticated");
        }

        return sessionId;
    }
}
