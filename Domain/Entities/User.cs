namespace Domain.Entities;

/// <summary>
/// Пользователь системы
/// </summary>
public class User : IFarmScoped
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }

    /// <inheritdoc />
    public Guid FarmId { get; set; }

    /// <summary>
    /// Ферма, к которой относится учетная запись
    /// </summary>
    public Farm? Farm { get; set; }

    /// <summary>
    /// Логин пользователя
    /// </summary>
    public string Login { get; set; } = string.Empty;
    
    /// <summary>
    /// Хеш пароля
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Роль в системе
    /// </summary>
    public Role Role { get; set; }
}

/// <summary>
/// Роль сотрудника в системе
/// </summary>
public enum Role
{
    User,
    Admin,
}