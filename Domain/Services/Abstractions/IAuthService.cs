using Domain.Dtos;

namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с аутентификацией пользователей
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Проверяет корректность логина и пароля от пользователя 
    /// </summary>
    /// <param name="loginData">Юзернейм и пароль от пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task LoginAsync(LoginDto loginData, CancellationToken cancellationToken = default);
}