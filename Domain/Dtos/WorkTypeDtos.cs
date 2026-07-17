namespace Domain.Dtos;

public record CreateWorkTypeRequest(
    string Name);

public record UpdateWorkTypeRequest(
    Guid Id,
    string? Name);