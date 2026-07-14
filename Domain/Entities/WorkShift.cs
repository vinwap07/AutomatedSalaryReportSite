using System.ComponentModel.DataAnnotations;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Запись о выполненной работе 
/// </summary>
public class WorkLog
{
    /// <summary>
    /// Уникальный идентификатор записи
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор сотрудника, выполнившего работу
    /// </summary>
    public Guid EmployeeId { get; set; }
    
    /// <summary>
    /// Сотрудник, выполнивший работу
    /// </summary>
    public Employee Employee { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор типа выполненной работы
    /// </summary>
    public Guid? WorkTypeId { get; set; }
    
    /// <summary>
    /// Тип выполненной работы (заполняется, если работа была выполнена)
    /// </summary>
    public WorkType? WorkType { get; set; }
    
    /// <summary>
    /// Дата выполнения работы (заполняется, если работа была выполнена)
    /// </summary>
    public DateOnly Date { get; set; }
    
    /// <summary>
    /// Количество отработанных часов (заполняется, если работа была выполнена)
    /// </summary>
    public int? WorkHours { get; set; }
    
    /// <summary>
    /// Итоговая сумма, расценка и объем выполненной работы (заполняется, если работа была выполнена)
    /// </summary>
    public WorkCost? WorkCost { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор причины отсутствия на работе (заполняется, если сотрудник отсутствовал)
    /// </summary>
    public Guid? ReasonForAbsenceId { get; set; }
    
    /// <summary>
    /// Причина отсутствия на работе (заполняется, если сотрудник отсутствовал)
    /// </summary>
    public ReasonForAbsence? ReasonForAbsence { get; set; }
}

