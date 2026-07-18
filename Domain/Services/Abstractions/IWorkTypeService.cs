using Domain.Dtos;
using Domain.Entities;

namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с типами работ
/// </summary>
public interface IWorkTypeService
{
    /// <summary>
    /// Создает новый тип работы в системе
    /// </summary>
    /// <param name="request">Данные о типе работы</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданный тип работы</returns>
    Task<WorkType> CreateAsync(CreateWorkTypeRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующий тип работы в системе
    /// </summary>
    /// <param name="request">Обновленные данные о типе работы</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленный тип работы</returns>
    Task<WorkType> UpdateAsync(UpdateWorkTypeRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет типа работы в системе по уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор удаляемого типа работы</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает тип работы из системы по его уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор типа работы</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Найденный тип работы</returns>
    Task<WorkType> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает все типы работ из системы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех типов работ из системы</returns>
    Task<IEnumerable<WorkType>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает все типы работ из системы, которые соответствуют фильтрам
    /// </summary>
    /// <param name="filters">Фильтры</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех типов работ из системы, которые соответствуют фильтрам</returns>
    Task<IEnumerable<WorkType>> GetByFiltersAsync(WorkTypeFilters filters, CancellationToken cancellationToken = default);
}