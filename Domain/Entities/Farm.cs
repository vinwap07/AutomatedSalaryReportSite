namespace Domain.Entities;

/// <summary>
/// Ферма (хозяйство). Фермы независимы друг от друга: работники, техника,
/// справочники и журнал работ одной фермы недоступны из другой
/// </summary>
public class Farm
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название фермы
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
