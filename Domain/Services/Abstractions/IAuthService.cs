namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с аутентификацией пользователей
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Проверяет корректность логина и пароля от пользователя 
    /// </summary>
    /// <param name="username">Логин</param>
    /// <param name="password">Пароль</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}