namespace Domain.Entities;

/// <summary>
/// Техника
/// </summary>
public class Equipment
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название техники
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Есть ли у техники трекер
    /// </summary>
    public bool HasTracker { get; set; }
}