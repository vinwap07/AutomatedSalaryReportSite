using Domain.Entities;

namespace Domain.Dtos;

public record EmployeeDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public Specialty Specialty { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
    public string? Comment { get; init; }
}

public record EmployeeListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Specialty Specialty { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
}

public record CreateEmployeeRequest
{
    public string Name { get; init; } = string.Empty;
    public Specialty Specialty { get; init; }
    public Guid EquipmentId { get; init; }
    public string? Comment { get; init; }
}
    
public record UpdateEmployeeRequest
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public Specialty? Specialty { get; init; } 
    public Guid? EquipmentId { get; init; }
    public string? Comment { get; init; }
}