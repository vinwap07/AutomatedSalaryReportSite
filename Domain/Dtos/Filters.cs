using Domain.Entities;

namespace Domain.Dtos;

public record Filters(
    int Page = 1,
    int PageSize = 50);

public record UserFilters(
    string? Login,
    Role? Role
    ) : Filters;

public record EmployeeFilters(
    string? Name,
    Specialty? Specialty, 
    Guid? EquipmentId
    ) : Filters;

public record WorkLogFilters(
    Guid? EmployeeId,
    DateOnly? From,
    DateOnly? To,
    bool? HasReasonForAbsence,
    Guid? WorkTypeId
    ) : Filters;    
public record EquipmentFilters(
    string? Name,
    bool? HasTracker
    ) : Filters; 

public record WorkTypeFilters(
    string? Name
    ) : Filters;

public record ReasonForAbsenceFilters(
    string? Name
    ) : Filters;