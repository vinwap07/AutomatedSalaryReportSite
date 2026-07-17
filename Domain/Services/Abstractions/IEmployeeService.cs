using Domain.Dtos;
using Domain.Entities;

namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с сотрудниками
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Создает нового сотрудника в системе
    /// </summary>
    /// <param name="request">Данные о создаваемом сотруднике</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданный сотрудник</returns>
    Task<EmployeeDetailsDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующего сотрудника в системе
    /// </summary>
    /// <param name="request">Обновленные данные о сотруднике</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленный сотрудник</returns>
    Task<EmployeeDetailsDto> UpdateAsync(UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет сотрудника из системы
    /// </summary>
    /// <param name="id">Уникальный идентификатор удаляемого сотрудника</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает сотрудника по его уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор сотрудника</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Найденный сотрудник</returns>
    Task<EmployeeDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает всех сотрудников из системы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция существующих сотрудников</returns>
    Task<IEnumerable<EmployeeListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает всех сотрудников из системы, которые соответствуют фильтрам
    /// </summary>
    /// <param name="filters">Фильтры</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция сотрудников, соответствующих фильтрам</returns>
    Task<IEnumerable<EmployeeListItemDto>> GetByFiltersAsync(EmployeeFilters filters, CancellationToken cancellationToken = default);
}