namespace Domain.Repositories;

/// <summary>
/// Реализует транзакционное сохранение операций
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Транзакционно сохраняет выполненные операции
    /// </summary>
    /// <param name="cancellationToken">Токен отмены транзакции</param>
    /// <returns>Количество измененных сущностей</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}