using Domain.Dtos;
using Domain.Entities;

namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с техникой
/// </summary>
public interface IEquipmentService
{
    /// <summary>
    /// Создает технику в системе
    /// </summary>
    /// <param name="request">Данные о создаваемой техники</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданная техника</returns>
    Task<Equipment> CreateAsync(CreateEquipmentRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующую технику в системе 
    /// </summary>
    /// <param name="request">Обновленные данные о технике</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленная техника</returns>
    Task<Equipment> UpdateAsync(UpdateEquipmentRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет технику из системы
    /// </summary>
    /// <param name="id">Уникальный идентификатор удаляемой техники</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает технику по ее уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор техники</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Найденная техника</returns>
    Task<Equipment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает всю технику из системы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех техник из системы</returns>
    Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает всю технику из системы, которая соответствуют фильтрам
    /// </summary>
    /// <param name="filters">Фильтры</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех техник из системы, которые соответствуют фильтрам</returns>
    Task<Equipment> GetByFiltersAsync(EquipmentFilters filters, CancellationToken cancellationToken = default);
}