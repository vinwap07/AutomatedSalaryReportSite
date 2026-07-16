using Domain.Dtos;

namespace Domain.Services.Abstractions;

public interface IUserService
{
    Task<UserDetailsDto> CreateAsync(CreateUserRequest user, CancellationToken cancellationToken = default);
    Task<UserDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserListItemDto>> GetByFiltersIdAsync(UserFilters filters, CancellationToken cancellationToken = default);
    Task<UserDetailsDto> UpdateAsync(UpdateUserRequest user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}