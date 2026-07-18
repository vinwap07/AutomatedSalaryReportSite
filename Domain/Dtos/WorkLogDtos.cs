using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Dtos;

public record WorkLogDetailsDto
{
    public Guid Id { get; init; }
    public EmployeeDetailsDto Employee { get; init; }
    public WorkType? WorkType { get; init; }
    public DateOnly Date { get; init; }
    public int? WorkHours { get; init; }
    public WorkCost? WorkCost { get; init; }
    public ReasonForAbsence? ReasonForAbsence { get; init; }
}

public record WorkLogListItemDto
{
    public Guid Id { get; init; }
    public EmployeeListItemDto Employee { get; init; } = null!; 
    public WorkType? WorkType { get; init; }
    public DateOnly Date { get; init; }
    public int? WorkHours { get; init; }
    public WorkCost? WorkCost { get; init; }
    public ReasonForAbsence? ReasonForAbsence { get; init; }
}

public record CreateWorkLogRequest
{
    public Guid EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public Guid? WorkTypeId { get; set; }
    public int? WorkHours { get; set; }
    public WorkCost? WorkCost { get; set; }
    public Guid? ReasonForAbsenceId { get; set; }
}

public record UpdateWorkLogRequest
{
    public Guid Id { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateOnly? Date { get; set; }
    public Guid? WorkTypeId { get; set; }
    public int? WorkHours { get; set; }
    public WorkCost? WorkCost { get; set; }
    public Guid? ReasonForAbsenceId { get; set; }
}
