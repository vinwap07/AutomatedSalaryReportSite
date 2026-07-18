namespace Domain.Dtos;

public record CreateReasonForAbsenceRequest
{
    public string Name { get; set; } = string.Empty;
}

public record UpdateReasonForAbsenceRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}