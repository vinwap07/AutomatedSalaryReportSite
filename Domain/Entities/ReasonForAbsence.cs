namespace Domain.Entities;

/// <summary>
/// Причина отсутствия сотрудника на работе
/// </summary>
public class ReasonForAbsence
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Причина отсутствия
    /// </summary>
    public string Name { get; set; } = string.Empty;
}