using System.Collections;
using Domain.Dtos;
using Domain.Entities;

namespace Domain.Services.Abstractions;

public interface IReasonForAbsenceService
{
    Task<ReasonForAbsence> GetReasonForAbsenceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReasonForAbsence> CreateReasonForAbsenceAsync(CreateReasonForAbsenceRequest reason, CancellationToken cancellationToken = default);
    Task<ReasonForAbsence> UpdateReasonForAbsenceAsync(UpdateReasonForAbsenceRequest reason, CancellationToken cancellationToken = default);
    Task DeleteReasonForAbsenceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReasonForAbsence>> GetAllReasonsForAbsenceAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ReasonForAbsence>> GetReasonsForAbsenceByFiltersAsync(ReasonForAbsenceFilters filters, CancellationToken cancellationToken = default);
}