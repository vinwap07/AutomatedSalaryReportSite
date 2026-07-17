using Domain.Dtos;
using Domain.Entities;
using Mapster;

namespace Domain.Mappers;

public static class WorkLogMappingExtensions
{
    public static WorkLogDetailsDto ToDetailsDto(this WorkLog workLog)
    {
        var dto = workLog.Adapt<WorkLogDetailsDto>();
        return dto with { Employee = workLog.Employee.ToDetailsDto() };
    }

    public static WorkLogListItemDto ToListItemDto(this WorkLog workLog)
    {
        var dto = workLog.Adapt<WorkLogListItemDto>();
        return dto with { Employee = workLog.Employee.ToListItemDto() };
    }

    public static WorkLog ToWorkLog(this CreateWorkLogRequest request)
    {
        return request.Adapt<WorkLog>();
    }

    public static void UpdateWorkLog(this UpdateWorkLogRequest request, WorkLog workLog)
    {
        request.Adapt(workLog);
    }
}