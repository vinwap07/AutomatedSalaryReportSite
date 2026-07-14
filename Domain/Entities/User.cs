namespace Domain.Entities;

/// <summary>
/// Пользователь системы
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }
    
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