using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Dtos;

public record WorkLogDetailsDto(
    Guid Id,
    EmployeeDetailsDto Employee,
    WorkType? WorkType,
    DateOnly Date,
    int? WorkHours,
    WorkCost? WorkCost,
    ReasonForAbsence? ReasonForAbsence);

public record WorkLogListItemDto(
    Guid Id,
    EmployeeDetailsDto Employee,
    WorkType? WorkType,
    DateOnly Date,
    int? WorkHours,
    WorkCost? WorkCost,
    ReasonForAbsence? ReasonForAbsence);

public record CreateWorkLogRequest(
    Guid EmployeeId,
    DateOnly Date,
    Guid? WorkTypeId,
    int? WorkHours,
    WorkCost? WorkCost,
    Guid? ReasonForAbsenceId);

public record UpdateWorkLogRequest(
    Guid Id,
    Guid EmployeeId,
    DateOnly Date,
    Guid? WorkTypeId,
    int? WorkHours,
    WorkCost? WorkCost,
    Guid? ReasonForAbsenceId);