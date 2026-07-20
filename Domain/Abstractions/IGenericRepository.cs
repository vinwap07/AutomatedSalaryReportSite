using System.Linq.Expressions;

namespace Domain.Repositories;

/// <summary>
/// Обобщенный репозиторий
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
/// <typeparam name="TKey">Тип первичного ключа сущности</typeparam>
public interface IGenericRepository<TEntity, in TKey> where TEntity : class
{
    /// <summary>
    /// Получает все существующие сущности
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <param name="includes">Пути навигационных свойств, которые нужно подгрузить (например "Employee.Equipment")</param>
    /// <returns>Коллекция всех существующих сущностей</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default, params string[] includes);
    
    /// <summary>
    /// Находит сущность по ее первичному ключу
    /// </summary>
    /// <param name="id">Первичный ключ сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Сущность или null, если сущность не найдена</returns>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Находит коллекцию сущностей, удовлетворяющих условию
    /// </summary>
    /// <param name="predicate">Условие</param>
    /// <param name="page">Номер страницы</param>
    /// <param name="pageSize">Количество записей на странице</param>>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <param name="includes">Пути навигационных свойств, которые нужно подгрузить (например "Employee.Equipment")</param>
    /// <returns>Коллекция найденных сущностей</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize, CancellationToken cancellationToken = default, params string[] includes);

    /// <summary>
    /// Создает новую сущность
    /// </summary>
    /// <param name="entity">Создаваемая сущность</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданная сущность</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет сущность по ее первичному ключу
    /// </summary>
    /// <param name="id">Первичный ключ сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Асинхронная операция</returns>
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}