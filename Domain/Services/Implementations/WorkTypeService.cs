using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

/// <inheritdoc />
public class WorkTypeService(
    IGenericRepository<WorkType, Guid> workTypeRepository,
    IUnitOfWork unitOfWork
    ) : IWorkTypeService
{
    /// <inheritdoc />
    public async Task<WorkType> CreateAsync(CreateWorkTypeRequest request, CancellationToken cancellationToken = default)
    {
        var workType = request.ToWorkType();
        workType.Id = Guid.NewGuid();
        await workTypeRepository.AddAsync(workType, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return workType;
    }

    /// <inheritdoc />
    public async Task<WorkType> UpdateAsync(UpdateWorkTypeRequest request, CancellationToken cancellationToken = default)
    {
        var workType = await workTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (workType == null)
        {
            throw new NotFoundException(nameof(workType), request.Id);
        }
        request.UpdateWorkType(workType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return workType;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await workTypeRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkType> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workType = await workTypeRepository.GetByIdAsync(id, cancellationToken);
        if (workType == null)
        {
            throw new NotFoundException(nameof(workType), id);
        }
        return workType;
    }

    /// <inheritdoc />
    public Task<IEnumerable<WorkType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return workTypeRepository.GetAllAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<WorkType>> GetByFiltersAsync(WorkTypeFilters filters, CancellationToken cancellationToken = default)
    {
        return workTypeRepository.FindAsync(w =>
                filters.Name == null || w.Name.Contains(filters.Name),
            filters.Page,
            filters.PageSize,
            cancellationToken);
        
    }
}