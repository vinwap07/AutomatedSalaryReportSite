namespace Domain.Services.Abstractions;

public interface IAuthService
{
    Task LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
}