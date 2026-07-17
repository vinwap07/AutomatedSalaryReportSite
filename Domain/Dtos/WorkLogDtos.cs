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
    public Guid EmployeeId { get; init; }
    public DateOnly Date { get; init; }
    public Guid? WorkTypeId { get; init; }
    public int? WorkHours { get; init; }
    public WorkCost? WorkCost { get; init; }
    public Guid? ReasonForAbsenceId { get; init; }
}

public record UpdateWorkLogRequest
{
    public Guid Id { get; init; }
    public Guid? EmployeeId { get; init; }
    public DateOnly? Date { get; init; }
    public Guid? WorkTypeId { get; init; }
    public int? WorkHours { get; init; }
    public WorkCost? WorkCost { get; init; }
    public Guid? ReasonForAbsenceId { get; init; }
}
