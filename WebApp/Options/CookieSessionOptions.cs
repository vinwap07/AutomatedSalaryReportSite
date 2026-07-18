namespace WebApp.Options;

/// <summary>
/// Настройки конфигурации пользовательской cookie-сессии
/// </summary>
public class CookieSessionOptions
{
    /// <summary>
    /// Название секции в файле конфигурации
    /// </summary>
    public const string SectionName = "Cookie";

    /// <summary>
    /// Имя cookie
    /// </summary>
    public required string CookieName { get; init; }

    /// <summary>
    /// Время жизни сессии в часах
    /// </summary>
    public int SessionLifetimeHours { get; init; } = 12;

    /// <summary>
    /// Запрет доступа к cookie из JavaScript
    /// </summary>
    public bool HttpOnly { get; init; } = true;

    /// <summary>
    /// Передавать cookie только по HTTPS
    /// </summary>
    public bool Secure { get; init; } = true;
}