using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

/// <inheritdoc />
public class WorkLogService(
    IGenericRepository<WorkLog, Guid> workLogRepository,
    IUnitOfWork unitOfWork
    ) : IWorkLogService
{
    private static readonly string[] Includes =
    [
        nameof(WorkLog.Employee),
        $"{nameof(WorkLog.Employee)}.{nameof(Employee.Equipment)}",
        nameof(WorkLog.WorkType),
        nameof(WorkLog.ReasonForAbsence),
    ];

    /// <inheritdoc />
    public async Task<WorkLogDetailsDto> CreateAsync(CreateWorkLogRequest request, CancellationToken cancellationToken = default)
    {
        var workLog = request.ToWorkLog();
        workLog.Id = Guid.NewGuid();
        await workLogRepository.AddAsync(workLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(workLog.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkLogDetailsDto> UpdateAsync(UpdateWorkLogRequest request, CancellationToken cancellationToken = default)
    {
        var workLog = await workLogRepository.GetByIdAsync(request.Id, cancellationToken);
        if (workLog == null)
        {
            throw new NotFoundException(nameof(workLog), request.Id);
        }
        request.UpdateWorkLog(workLog);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(workLog.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid workLogId, CancellationToken cancellationToken = default)
    {
        await workLogRepository.DeleteAsync(workLogId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkLogDetailsDto> GetByIdAsync(Guid workLogId, CancellationToken cancellationToken = default)
    {
        var workLogs = await workLogRepository.FindAsync(w => w.Id == workLogId, 1, 1, cancellationToken, Includes);
        var workLog = workLogs.FirstOrDefault();
        if (workLog == null)
        {
            throw new NotFoundException(nameof(workLog), workLogId);
        }
        return workLog.ToDetailsDto();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WorkLogListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var workLogs = await workLogRepository.GetAllAsync(cancellationToken, Includes);
        return workLogs.Select(x => x.ToListItemDto());
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WorkLogListItemDto>> GetByFiltersAsync(WorkLogFilters filters, CancellationToken cancellationToken = default)
    {
        var workLogs = await workLogRepository.FindAsync(w =>
                (filters.EmployeeId == null || w.EmployeeId == filters.EmployeeId) &&
                (filters.From == null || w.Date >= filters.From) &&
                (filters.To == null || w.Date <= filters.To) &&
                (filters.HasReasonForAbsence == null || ((w.ReasonForAbsence != null) == filters.HasReasonForAbsence)) &&
                (filters.WorkTypeId == null || w.WorkTypeId == filters.WorkTypeId),
            filters.Page,
            filters.PageSize,
            cancellationToken,
            Includes);
        return workLogs.Select(x => x.ToListItemDto());
    }
}
