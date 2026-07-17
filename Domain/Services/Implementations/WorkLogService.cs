using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class WorkLogService(
    IGenericRepository<WorkLog, Guid> workLogRepository,
    IUnitOfWork unitOfWork
    ) : IWorkLogService
{
    public async Task<WorkLogDetailsDto> CreateAsync(CreateWorkLogRequest request, CancellationToken cancellationToken = default)
    {
        var workLog = request.ToWorkLog();
        workLog.Id = Guid.NewGuid();
        await workLogRepository.AddAsync(workLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return workLog.ToDetailsDto();
    }

    public async Task<WorkLogDetailsDto> UpdateAsync(UpdateWorkLogRequest request, CancellationToken cancellationToken = default)
    {
        var workLog = await workLogRepository.GetByIdAsync(request.Id, cancellationToken);
        if (workLog == null)
        {
            throw new NotFoundException(nameof(workLog), request.Id);
        }
        request.UpdateWorkLog(workLog);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return workLog.ToDetailsDto();
    }

    public async Task DeleteAsync(Guid workLogId, CancellationToken cancellationToken = default)
    {
        await workLogRepository.DeleteAsync(workLogId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkLogDetailsDto> GetByIdAsync(Guid workLogId, CancellationToken cancellationToken = default)
    {
        var workLog = await workLogRepository.GetByIdAsync(workLogId, cancellationToken);
        if (workLog == null)
        {
            throw new NotFoundException(nameof(workLog), workLogId);
        }
        return workLog.ToDetailsDto();
    }

    public async Task<IEnumerable<WorkLogListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var workLogs = await workLogRepository.GetAllAsync(cancellationToken);
        return workLogs.Select(x => x.ToListItemDto());
    }

    public Task<IEnumerable<WorkLogListItemDto>> GetByFiltersAsync(WorkLogFilters filters, CancellationToken cancellationToken = default)
    {
        
    }
}