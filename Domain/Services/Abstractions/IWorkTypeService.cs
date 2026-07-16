using Domain.Dtos;
using Domain.Entities;

namespace Domain.Services.Abstractions;

public interface IWorkTypeService
{
    Task<WorkType> GetWorkTypeByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkType>> GetAllWorkTypesAsync(CancellationToken cancellationToken = default);
    Task<WorkType> CreateWorkTypeAsync(CreateWorkTypeDto dto, CancellationToken cancellationToken = default);
    Task<WorkType> UpdateWorkTypeAsync(Guid id, UpdateWorkTypeDto dto, CancellationToken cancellationToken = default);
    Task DeleteWorkTypeAsync(Guid id, CancellationToken cancellationToken = default);
    Task GetWorkTypeByFiltersAsync(WorkTypeFilters filters, CancellationToken cancellationToken = default);
}