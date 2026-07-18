using System.Net;
using Domain.Dtos;

namespace WebApp.Session.Service;

public interface ISessionService
{
    /// <summary>
    /// Создаёт и сохраняет пользовательскую сессию
    /// </summary>
    /// <param name="user">Данные пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SignInAsync(UserDetailsDto user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет пользовательскую сессию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SignOutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает идентификатор пользователя из текущей сессии
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Идентификатор пользователя или null</returns>
    Task<Guid?> GetUserIdAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает информацию о пользователе из текущей сессии
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информацию о пользователе или null</returns>
    Task<SessionDataDto?> GetSessionDataAsync(CancellationToken cancellationToken = default);
}