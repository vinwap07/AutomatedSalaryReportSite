using Domain.Dtos;
using Domain.Entities;
using Mapster;

namespace Domain.Mappers;

public static class EquipmentMappingExtensions
{
    public static Equipment ToEquipment(this CreateEquipmentRequest request)
    {
        return request.Adapt<Equipment>();
    }

    public static void UpdateEquipment(this UpdateEquipmentRequest request, Equipment equipment)
    {
        request.Adapt(equipment);
    }
}