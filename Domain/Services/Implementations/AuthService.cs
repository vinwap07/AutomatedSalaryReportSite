using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Domain.Services.Implementations;

public class AuthService(
    IPasswordHasher<User> passwordHasher,
    IGenericRepository<User, Guid> userRepository
    ) : IAuthService
{
    public async Task LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var users = (await userRepository.FindAsync(user => user.Login == username, cancellationToken))
            .ToList();
        if (users.Count == 0)
        {
            throw new UnauthorizedException("Wrong username or password");
        }
        
        var user = users[0];
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("Wrong username or password");
        }
    }
}