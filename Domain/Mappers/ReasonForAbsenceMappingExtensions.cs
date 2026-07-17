using Domain.Dtos;
using Domain.Entities;
using Mapster;

namespace Domain.Mappers;

public static class ReasonForAbsenceMappingExtensions
{
    public static ReasonForAbsence ToReasonForAbsence(this CreateReasonForAbsenceRequest request)
    {
        return request.Adapt<ReasonForAbsence>();
    }

    public static void UpdateReasonForAbsence(this UpdateReasonForAbsenceRequest request, ReasonForAbsence entity)
    {
        request.Adapt(entity);
    }
}