using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ApplicationService;

public class UserManager : IUserManager
{
    private readonly HttpContextAccessor _httpContextAccessor;

    public UserManager(HttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Create()
    {
        var submissionId = Guid.NewGuid().ToString();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, submissionId),
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

        return submissionId;
    }

    public string GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        if (userId == null)
        {
            throw new Exception("User is not authenticated");
        }

        return userId;
    }
}
