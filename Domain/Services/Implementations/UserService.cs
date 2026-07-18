using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Domain.Services.Implementations;

public class UserService(
    IPasswordHasher<User> passwordHasher,
    IGenericRepository<User, Guid> userRepository,
    IUnitOfWork unitOfWork
    ): IUserService
{
    public async Task<UserDetailsDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = request.ToUser();
        user.Id = Guid.NewGuid();
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return user.ToDetailsDto();
    }

    public async Task<UserDetailsDto> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(user), request.Id);
        }
        request.UpdateUser(user);
        if (request.Password is not null)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return user.ToDetailsDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await userRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(nameof(user), id);
        }
        
        return user.ToDetailsDto();
    }

    public async Task<IEnumerable<UserListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(u => u.ToListItemDto());
    }

    public async Task<IEnumerable<UserListItemDto>> GetByFiltersIdAsync(UserFilters filters, CancellationToken cancellationToken = default)
    {
        var users = await userRepository.FindAsync(u => 
                (filters.Login == null || u.Login.Contains(filters.Login)) &&
                (filters.Role == null || u.Role == filters.Role),
            filters.Page,
            filters.PageSize,
            cancellationToken);
        return users.Select(u => u.ToListItemDto());
    }
}