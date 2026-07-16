using Domain.Dtos;
using Domain.Entities;

namespace Domain.Services.Abstractions;

public interface IEquipmentService
{
    Task<Equipment> GetEquipmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Equipment>> GetAllEquipmentsAsync(CancellationToken cancellationToken = default);
    Task<Equipment> CreateEquipmentAsync(CreateEquipmentRequest equipment, CancellationToken cancellationToken = default);
    Task<Equipment> UpdateEquipmentAsync(UpdateEquipmentRequest equipment, CancellationToken cancellationToken = default);
    Task DeleteEquipmentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Equipment> GetEquipmentsByFiltersAsync(EquipmentFilters filters, CancellationToken cancellationToken = default);
}