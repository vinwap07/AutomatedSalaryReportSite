namespace Domain.Dtos;

public record CreateWorkTypeRequest
{
    public string Name { get; set; } = string.Empty;
}

public record UpdateWorkTypeRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}
