using Domain.Entities;

namespace Domain.Dtos;

public record CreateWorkTypeRequest
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Null — специальность не назначена</summary>
    public Specialty? Specialty { get; set; }
}

public record UpdateWorkTypeRequest
{
    public Guid Id { get; set; }

    /// <summary>Null — оставить прежнее название</summary>
    public string? Name { get; set; }

    /// <summary>Перезаписывается всегда: null снимает привязку к специальности</summary>
    public Specialty? Specialty { get; set; }
}
