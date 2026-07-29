namespace Domain.Entities;

/// <summary>
/// Сущность, принадлежащая конкретной ферме.
/// Такие сущности автоматически фильтруются по ферме текущего пользователя
/// </summary>
public interface IFarmScoped
{
    /// <summary>
    /// Уникальный идентификатор фермы, которой принадлежит запись
    /// </summary>
    Guid FarmId { get; set; }
}
