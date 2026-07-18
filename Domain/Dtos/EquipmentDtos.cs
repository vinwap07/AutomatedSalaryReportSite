namespace Domain.Dtos;

public record CreateEquipmentRequest
{
    public string Name { get; set; } = string.Empty;
    public bool HasTracker { get; set; }
}

public record UpdateEquipmentRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool? HasTracker { get; set; }
}