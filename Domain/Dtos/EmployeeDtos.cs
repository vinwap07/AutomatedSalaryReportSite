using Domain.Entities;

namespace Domain.Dtos;

public record EmployeeDetailsDto(Guid Id, string FirstName, string LastName, Guid? UserId,
    Specialty Specialty, string EquipmentName, string? Comment, string? Surname);

public record EmployeeListItemDto(Guid Id, string FirstName, string LastName, 
    string? Surname, Specialty Specialty, string EquipmentName);

public record CreateEmployeeRequest(string FirstName, string LastName, 
    Specialty Specialty, Guid EquipmentId, string? Comment, string? Surname);
    
public record UpdateEmployeeRequest(Guid Id, string FirstName, string LastName, 
    Specialty Specialty, Guid EquipmentId, string? Comment, string? Surname);