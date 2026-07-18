using Domain.Dtos;
using Microsoft.Extensions.Options;
using WebApp.Options;
using WebApp.Session.Store;

namespace WebApp.Session.Service;

/// <inheritdoc />
public class SessionService(
    IHttpContextAccessor httpContextAccessor,
    ISessionStore sessionStore,
    IOptions<CookieSessionOptions> options
    ) :ISessionService
{
    private CookieSessionOptions? _optionsValue = options?.Value;
    
    /// <inheritdoc />
    public async Task SignInAsync(UserDetailsDto user, CancellationToken cancellationToken = default)
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null)
        {
            return;
        }
        
        var sessionKey = Guid.NewGuid().ToString("N");
        var expiresAt = DateTimeOffset.UtcNow.AddHours(_optionsValue.SessionLifetimeHours);

        var sessionData = new SessionDataDto
        {
            UserId = user.Id,
            Login = user.Login,
            Role = user.Role,
            ExpiresAt = expiresAt,
        };
        
        await sessionStore.SaveAsync(sessionKey, sessionData, cancellationToken);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = _optionsValue.HttpOnly,
            Secure = _optionsValue.Secure,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt,
        };
        
        context.Response.Cookies.Append(_optionsValue.CookieName, sessionKey, cookieOptions);
    }

    /// <inheritdoc />
    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null)
        {
            return;
        }
        
        if (context.Request.Cookies.TryGetValue(_optionsValue.CookieName, out var sessionKey) &&
            !string.IsNullOrWhiteSpace(sessionKey))
        {
            await sessionStore.RemoveAsync(sessionKey, cancellationToken);
        }
        
        context.Response.Cookies.Delete(_optionsValue.CookieName);
    }

    /// <inheritdoc />
    public async Task<Guid?> GetUserIdAsync(CancellationToken cancellationToken = default)
    {
        var context = httpContextAccessor.HttpContext;
        if (context == null)
        {
            return null;
        }
        
        if (!context.Request.Cookies.TryGetValue(_optionsValue.CookieName, out var sessionKey) ||
            string.IsNullOrWhiteSpace(sessionKey))
        {
            return null;
        }

        var sessionData = await sessionStore.GetAsync(sessionKey, cancellationToken);

        return sessionData?.UserId;
    }

    /// <inheritdoc />
    public async Task<SessionDataDto?> GetSessionDataAsync(CancellationToken cancellationToken = default)
    {
        var context = httpContextAccessor.HttpContext;
        if (context is null)
        {
            return null;
        }

        if (!context.Request.Cookies.TryGetValue(_optionsValue.CookieName, out var sessionKey) ||
            string.IsNullOrWhiteSpace(sessionKey))
        {
            return null;
        }

        var sessionData = await sessionStore.GetAsync(sessionKey, cancellationToken);

        return sessionData;
    }
}