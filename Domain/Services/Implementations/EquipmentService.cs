using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class EquipmentService(
    IGenericRepository<Equipment, Guid> equipmentRepository,
    IUnitOfWork unitOfWork
    ) : IEquipmentService
{
    public async Task<Equipment> CreateAsync(CreateEquipmentRequest request, CancellationToken cancellationToken = default)
    {
        var equipment = request.ToEquipment();
        equipment.Id = Guid.NewGuid();
        await equipmentRepository.AddAsync(equipment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return equipment;
    }

    public async Task<Equipment> UpdateAsync(UpdateEquipmentRequest request, CancellationToken cancellationToken = default)
    {
        var equipment = await equipmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (equipment == null)
        {
            throw new NotFoundException(nameof(equipment), request.Id);
        }
        request.UpdateEquipment(equipment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return equipment;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await equipmentRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Equipment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var equipment = await equipmentRepository.GetByIdAsync(id, cancellationToken);
        if (equipment == null)
        {
            throw new NotFoundException(nameof(equipment), id);
        }
        return equipment;
    }

    public Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return equipmentRepository.GetAllAsync(cancellationToken);
    }

    public Task<IEnumerable<Equipment>> GetByFiltersAsync(EquipmentFilters filters, CancellationToken cancellationToken = default)
    {
        return equipmentRepository.FindAsync(e => 
                (filters.Name == null || e.Name.Contains(filters.Name)) && 
                (filters.HasTracker == null || e.HasTracker == filters.HasTracker), 
            filters.Page, 
            filters.PageSize, 
            cancellationToken);
    }
}