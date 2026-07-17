using Domain.Dtos;
using Domain.Entities;
using Domain.Services.Implementations;
using Mapster;

namespace Domain.Mappers;

public static class WorkTypeMappingExtensions
{
    public static WorkType ToWorkType(this CreateWorkTypeRequest request)
    {
        return request.Adapt<WorkType>();
    }

    public static void UpdateWorkType(this UpdateWorkTypeRequest request, WorkType workType)
    {
        request.Adapt(workType);
    }
}