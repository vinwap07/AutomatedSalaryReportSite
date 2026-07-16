using Domain.Entities;

namespace Domain.Dtos;

public record Filters(int Page = 1, int PageSize = 10);

public record UserFilters(string? Login) : Filters;

public record EmployeeFilters(string? Name, Specialty? Specialty, 
    Guid? EquipmentId) : Filters;
    
public record EquipmentFilters(string? Name, bool? HasTracker) : Filters; 

public record WorkTypeFilters(string? Name) : Filters;

public record ReasonForAbsenceFilters(string? Name) : Filters;