using Domain.Entities;

namespace Domain.Dtos;

public record Filters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public record UserFilters : Filters
{
    public string? Login { get; set; }
    public Role? Role { get; set; }
}

public record EmployeeFilters : Filters
{
    public string? Name { get; set; }
    public Specialty? Specialty { get; set; }
    public Guid? EquipmentId { get; set; }
    public Guid? UserId { get; set; }
}

public record WorkLogFilters : Filters
{
    public Guid? EmployeeId { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public bool? HasReasonForAbsence { get; set; }
    public Guid? WorkTypeId { get; set; }
}

public record EquipmentFilters : Filters
{
    public string? Name { get; set; }
    public bool? HasTracker { get; set; }
}

public record WorkTypeFilters : Filters
{
    public string? Name { get; set; }
}

public record ReasonForAbsenceFilters : Filters
{
    public string? Name { get; set; }
}