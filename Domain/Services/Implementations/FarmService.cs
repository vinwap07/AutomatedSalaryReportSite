using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

/// <inheritdoc />
public class FarmService(
    IGenericRepository<Farm, Guid> farmRepository,
    IUnitOfWork unitOfWork
    ) : IFarmService
{
    /// <inheritdoc />
    public async Task<Farm> EnsureAsync(Guid id, string name, CancellationToken cancellationToken = default)
    {
        var farm = await farmRepository.GetByIdAsync(id, cancellationToken);
        if (farm == null)
        {
            farm = new Farm { Id = id, Name = name };
            await farmRepository.AddAsync(farm, cancellationToken);
        }
        else if (farm.Name != name)
        {
            farm.Name = name;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return farm;
    }

    /// <inheritdoc />
    public async Task<Farm> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var farm = await farmRepository.GetByIdAsync(id, cancellationToken);
        if (farm == null)
        {
            throw new NotFoundException(nameof(Farm), id);
        }
        return farm;
    }

    /// <inheritdoc />
    public Task<IEnumerable<Farm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return farmRepository.GetAllAsync(cancellationToken);
    }
}
