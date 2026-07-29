using Domain.Entities;

namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с фермами
/// </summary>
public interface IFarmService
{
    /// <summary>
    /// Создает ферму с заданным идентификатором, если ее еще нет, иначе обновляет название
    /// </summary>
    /// <param name="id">Уникальный идентификатор фермы</param>
    /// <param name="name">Название фермы</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданная или существующая ферма</returns>
    Task<Farm> EnsureAsync(Guid id, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает ферму по ее уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор фермы</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Найденная ферма</returns>
    Task<Farm> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все фермы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех ферм</returns>
    Task<IEnumerable<Farm>> GetAllAsync(CancellationToken cancellationToken = default);
}
