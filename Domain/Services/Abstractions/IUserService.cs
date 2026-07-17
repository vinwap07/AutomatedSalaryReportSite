using Domain.Dtos;

namespace Domain.Services.Abstractions;

/// <summary>
/// Сервис для работы с пользователями
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Создает нового пользователя в системе
    /// </summary>
    /// <param name="request">Данные о пользователе</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданный пользователь</returns>
    Task<UserDetailsDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующего пользователя в системе
    /// </summary>
    /// <param name="request">Обновленные данные о пользователе</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленный пользователь</returns>
    Task<UserDetailsDto> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет пользователя из системы по уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор удаляемого пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает пользователя по его уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Найденный пользователь</returns>
    Task<UserDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает всех пользователей из системы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех пользователей из системы</returns>
    Task<IEnumerable<UserListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает всех пользователей из системы, которые соответствуют фильтрам
    /// </summary>
    /// <param name="filters">Фильтры</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех пользователей из системы, которые соответствуют фильтрам</returns>
    Task<IEnumerable<UserListItemDto>> GetByFiltersIdAsync(UserFilters filters, CancellationToken cancellationToken = default);
}