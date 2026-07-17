using Domain.Entities;

namespace Domain.Dtos;

public record EmployeeDetailsDto(
    Guid Id,
    string Name,
    Guid? UserId,
    Specialty Specialty,
    string EquipmentName,
    string? Comment);

public record EmployeeListItemDto(
    Guid Id,
    string Name,
    Specialty Specialty,
    string EquipmentName);

public record CreateEmployeeRequest(
    string Name,
    Specialty Specialty,
    Guid EquipmentId,
    string? Comment);
    
public record UpdateEmployeeRequest(Guid Id,
    string Name, 
    Specialty Specialty,
    Guid EquipmentId,
    string? Comment);