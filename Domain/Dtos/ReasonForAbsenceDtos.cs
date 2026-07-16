namespace Domain.Dtos;

public record CreateReasonForAbsenceRequest(string Name);

public record UpdateReasonForAbsenceRequest(Guid Id, string Name);