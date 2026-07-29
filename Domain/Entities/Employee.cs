using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Работник
/// </summary>
public class Employee : IFarmScoped
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }

    /// <inheritdoc />
    public Guid FarmId { get; set; }

    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор учетной записи (null, если у сотрудника нет доступа в систему).
    /// </summary>
    public Guid? UserId { get; set; }
    
    /// <summary>
    /// Учетная запись сотрудника.
    /// </summary>
    public User? User { get; set; }
    
    /// <summary>
    /// Специализация
    /// </summary>
    public Specialty Specialty { get; set; } 
    
    /// <summary>
    /// Комментарий по сотруднику
    /// </summary>
    public string? Comment { get; set; }
}

/// <summary>
/// Специализация сотрудника
/// </summary>
public enum Specialty
{
    [Display(Name = "Механизатор")]
    MachineOperator,
    
    [Display(Name = "Вахта")]
    Rotation,
    
    [Display(Name = "Водитель")]
    Driver,
    
    [Display(Name = "Слесарь")]
    Fitter,
    
    [Display(Name = "Специалист")]
    Specialist,
}