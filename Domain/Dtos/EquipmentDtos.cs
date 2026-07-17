namespace Domain.Dtos;

public record CreateEquipmentRequest(
    string Name,
    bool HasTracker);

public record UpdateEquipmentRequest(
    Guid Id,
    string? Name,
    bool? HasTracker);