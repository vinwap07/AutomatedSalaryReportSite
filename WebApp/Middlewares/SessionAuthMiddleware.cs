using Domain.Entities;
using WebApp.Session.Service;

namespace WebApp.Middlewares;

/// <summary>
/// Middleware авторизации по cookie-сессии: кладет данные сессии в HttpContext.Items,
/// перенаправляет неавторизованных на страницу входа и ограничивает /Admin ролью администратора
/// </summary>
public class SessionAuthMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Ключ, по которому данные сессии доступны в HttpContext.Items
    /// </summary>
    public const string SessionItemKey = "SessionData";

    private static readonly string[] PublicPathPrefixes =
    [
        "/Login",
        "/Error",
        "/css",
        "/js",
        "/lib",
        "/favicon.ico",
        "/WebApp.styles.css",
    ];

    public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
    {
        var sessionData = await sessionService.GetSessionDataAsync(context.RequestAborted);
        if (sessionData != null)
        {
            context.Items[SessionItemKey] = sessionData;
        }

        var path = context.Request.Path;
        if (IsPublicPath(path))
        {
            await next(context);
            return;
        }

        if (sessionData == null)
        {
            var returnUrl = context.Request.Path + context.Request.QueryString;
            context.Response.Redirect($"/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            return;
        }

        if (path.StartsWithSegments("/Admin") && sessionData.Role != Role.Admin)
        {
            context.Response.Redirect("/Reports");
            return;
        }

        await next(context);
    }

    private static bool IsPublicPath(PathString path)
    {
        return PublicPathPrefixes.Any(prefix => path.StartsWithSegments(prefix));
    }
}

/// <summary>
/// Расширения для получения данных сессии из HttpContext
/// </summary>
public static class SessionAuthExtensions
{
    /// <summary>
    /// Возвращает данные текущей сессии или null, если пользователь не авторизован
    /// </summary>
    public static SessionDataDto? GetSessionData(this HttpContext context)
    {
        return context.Items.TryGetValue(SessionAuthMiddleware.SessionItemKey, out var value)
            ? value as SessionDataDto
            : null;
    }
}
