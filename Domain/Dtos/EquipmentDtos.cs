namespace Domain.Dtos;

public record CreateEquipmentRequest
{
    public string Name { get; init; }
    public bool HasTracker { get; init; }
}

public record UpdateEquipmentRequest
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public bool? HasTracker { get; init; }
}