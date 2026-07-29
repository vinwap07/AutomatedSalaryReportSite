namespace Domain.Entities;

/// <summary>
/// Тип выполняемой работы
/// </summary>
public class WorkType : IFarmScoped
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }

    /// <inheritdoc />
    public Guid FarmId { get; set; }

    /// <summary>
    /// Название типа выполняемой работы
    /// </summary>
    public string Name { get; set; } = string.Empty;
}