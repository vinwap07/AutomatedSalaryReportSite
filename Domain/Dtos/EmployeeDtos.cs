using Domain.Entities;

namespace Domain.Dtos;

public record EmployeeDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public string? UserLogin { get; init; }
    public Specialty Specialty { get; init; }
    public Guid? EquipmentId { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
    public string? Comment { get; init; }
}

public record EmployeeListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public string? UserLogin { get; init; }
    public Specialty Specialty { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
    public string? Comment { get; init; }
}

public record CreateEmployeeRequest
{
    public string Name { get; set; } = string.Empty;
    public Specialty Specialty { get; set; }
    public Guid? EquipmentId { get; set; }
    public string? Comment { get; set; }
}

public record UpdateEmployeeRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Specialty? Specialty { get; set; }
    public Guid? EquipmentId { get; set; }
    public string? Comment { get; set; }
}
