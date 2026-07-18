using Domain.Entities;

namespace WebApp.Session.Service;

/// <summary>
/// DTO данных пользовательской сессии
/// </summary>
public record SessionDataDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Роль пользователя
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// Дата и время окончания сессии
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }
}