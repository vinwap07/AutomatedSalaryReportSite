namespace Domain.Dtos;

public record CreateWorkTypeDto(string Name);

public record UpdateWorkTypeDto(Guid Id, string Name);