using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class ReasonForAbsenceService(
    IGenericRepository<ReasonForAbsence, Guid> reasonForAbsenceRepository,
    IUnitOfWork unitOfWork
    ) : IReasonForAbsenceService
{
    public async Task<ReasonForAbsence> CreateAsync(CreateReasonForAbsenceRequest request, CancellationToken cancellationToken = default)
    {
        var reason = request.ToReasonForAbsence();
        reason.Id = Guid.NewGuid();
        await reasonForAbsenceRepository.AddAsync(reason, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return reason;
    }

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

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await reasonForAbsenceRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ReasonForAbsence> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reason = await reasonForAbsenceRepository.GetByIdAsync(id, cancellationToken);
        if (reason == null)
        {
            throw new NotFoundException(nameof(reason), id);
        }
        return reason;
    }

    public Task<IEnumerable<ReasonForAbsence>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return reasonForAbsenceRepository.GetAllAsync(cancellationToken);
    }

    public Task<IEnumerable<ReasonForAbsence>> GetByFiltersAsync(ReasonForAbsenceFilters filters, CancellationToken cancellationToken = default)
    {
        
    }
}