using Domain.Dtos;

namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с выполненными работами 
/// </summary>
public interface IWorkLogService
{
    /// <summary>
    /// Создает новый отчет о выполненной работе в системе
    /// </summary>
    /// <param name="workLog">Данные о выполненной работе</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданный отчет</returns>
    Task<WorkLogDetailsDto> CreateAsync(CreateWorkLogRequest workLog, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующий отчет о выполненной работе в системе
    /// </summary>
    /// <param name="workLog">Обновленная информация о выполненной работе</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленный отчет</returns>
    Task<WorkLogDetailsDto> UpdateAsync(UpdateWorkLogRequest workLog, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет отчет из системы по уникальному идентификатору
    /// </summary>
    /// <param name="workLogId">Уникальный идентификатор удаляемого отчета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task DeleteAsync(Guid workLogId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает отчет о выполненной работе по уникальному идентификатору
    /// </summary>
    /// <param name="workLogId">Уникальный идентификатор отчета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Найденный отчет о выполненной работе</returns>
    Task<WorkLogDetailsDto> GetByIdAsync(Guid workLogId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает все отчеты о выполненных работах из системы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех отчетов о выполненной работе из системы</returns>
    Task<IEnumerable<WorkLogListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает все отчеты о выполненной работе из системы, которые соответствуют фильтрам
    /// </summary>
    /// <param name="filters">Фильтры</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех отчетов о выполненной работе из системы, которые соответствуют фильтрам</returns>
    Task<IEnumerable<WorkLogListItemDto>> GetByFiltersAsync(WorkLogFilters filters, CancellationToken cancellationToken = default);
}