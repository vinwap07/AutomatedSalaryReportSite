using System.Collections;
using Domain.Dtos;
using Domain.Entities;

namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с причинами отсутствия на работе
/// </summary>
public interface IReasonForAbsenceService
{
    /// <summary>
    /// Создает новую причину отсутствия в системе
    /// </summary>
    /// <param name="reason">Данные о причине отсутствия</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданная причина отсутствия</returns>
    Task<ReasonForAbsence> CreateAsync(CreateReasonForAbsenceRequest reason, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующую причину отсутствия в системе
    /// </summary>
    /// <param name="reason">Обновленные данные о причине отсутствия</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленная причина отсутствия</returns>
    Task<ReasonForAbsence> UpdateAsync(UpdateReasonForAbsenceRequest reason, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет причину отсутствия в системе по уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор удаляемой причины</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает причину отсутствия по ее уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор причины отсутствия</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Найденная причина отсутствия</returns>
    Task<ReasonForAbsence> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает все причины отсутствия из системы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех техник из системы</returns>
    Task<IEnumerable<ReasonForAbsence>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает все причины отсутствия из системы, которые соответствуют фильтрам
    /// </summary>
    /// <param name="filters">Фильтры</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех причин отсутствия из системы, которые соответствуют фильтрам</returns>
    Task<IEnumerable<ReasonForAbsence>> GetByFiltersAsync(ReasonForAbsenceFilters filters, CancellationToken cancellationToken = default);
}