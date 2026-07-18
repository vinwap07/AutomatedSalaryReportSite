using WebApp.Session.Service;

namespace WebApp.Session.Store;

/// <summary>
/// Хранилище пользовательских сессий
/// </summary>
public interface ISessionStore
{
    /// <summary>
    /// Сохраняет данные пользовательской сессии по ключу
    /// </summary>
    /// <param name="key">Ключ сессии</param>
    /// <param name="sessionData">Данные сессии</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SaveAsync(string key, SessionDataDto sessionData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает данные пользовательской сессии по ключу
    /// </summary>
    /// <param name="key">Ключ сессии</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Данные сессии или null</returns>
    Task<SessionDataDto?> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет данные пользовательской сессии по ключу
    /// </summary>
    /// <param name="key">Ключ сессии</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}