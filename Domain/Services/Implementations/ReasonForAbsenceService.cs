using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

/// <inheritdoc />
public class ReasonForAbsenceService(
    IGenericRepository<ReasonForAbsence, Guid> reasonForAbsenceRepository,
    IUnitOfWork unitOfWork
    ) : IReasonForAbsenceService
{
    /// <inheritdoc />
    public async Task<ReasonForAbsence> CreateAsync(CreateReasonForAbsenceRequest request, CancellationToken cancellationToken = default)
    {
        var reason = request.ToReasonForAbsence();
        reason.Id = Guid.NewGuid();
        await reasonForAbsenceRepository.AddAsync(reason, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return reason;
    }

    /// <inheritdoc />
    public async Task<ReasonForAbsence> UpdateAsync(UpdateReasonForAbsenceRequest request, CancellationToken cancellationToken = default)
    {
        var reason = await reasonForAbsenceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (reason == null)
        {
            throw new NotFoundException(nameof(reason), request.Id);
        }
        request.UpdateReasonForAbsence(reason);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return reason;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await reasonForAbsenceRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ReasonForAbsence> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reason = await reasonForAbsenceRepository.GetByIdAsync(id, cancellationToken);
        if (reason == null)
        {
            throw new NotFoundException(nameof(reason), id);
        }
        return reason;
    }

    /// <inheritdoc />
    public Task<IEnumerable<ReasonForAbsence>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return reasonForAbsenceRepository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<ReasonForAbsence>> GetByFiltersAsync(ReasonForAbsenceFilters filters, CancellationToken cancellationToken = default)
    {
        return reasonForAbsenceRepository.FindAsync(r => 
                filters.Name == null || r.Name.Contains(filters.Name),
            filters.Page,
            filters.PageSize,
            cancellationToken);
    }
}