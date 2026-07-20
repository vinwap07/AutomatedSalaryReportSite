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
        // Сотрудник и дата обязательны, поэтому null означает "не менять".
        // Остальные поля опциональны у самой записи: null перезаписывает значение,
        // чтобы можно было, например, заменить работу на причину отсутствия.
        if (request.EmployeeId.HasValue)
        {
            workLog.EmployeeId = request.EmployeeId.Value;
        }
        if (request.Date.HasValue)
        {
            workLog.Date = request.Date.Value;
        }
        workLog.WorkTypeId = request.WorkTypeId;
        workLog.WorkHours = request.WorkHours;
        workLog.WorkCost = request.WorkCost;
        workLog.ReasonForAbsenceId = request.ReasonForAbsenceId;
    }
}