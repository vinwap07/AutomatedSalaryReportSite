using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Domain.Services.Implementations;

/// <inheritdoc />
public class AuthService(
    IPasswordHasher<User> passwordHasher,
    IGenericRepository<User, Guid> userRepository
    ) : IAuthService
{
    /// <inheritdoc />
    public async Task<UserDetailsDto> LoginAsync(LoginData loginData, CancellationToken cancellationToken = default)
    {
        var users = (await userRepository.FindAsync(user => user.Login == loginData.Username, 1, 1, cancellationToken))
            .ToList();
        if (users.Count == 0)
        {
            throw new UnauthorizedException("Wrong username or password");
        }
        
        var user = users[0];
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginData.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("Wrong username or password");
        }
        return user.ToDetailsDto();
    }
}