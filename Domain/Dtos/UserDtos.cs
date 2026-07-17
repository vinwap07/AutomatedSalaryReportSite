using Domain.Entities; 

namespace Domain.Dtos;

public record UserDetailsDto
{
    public Guid Id { get; init; }
    public string Login { get; init; } = string.Empty;
}

public record UserListItemDto
{
    public Guid Id { get; init; }
    public string Login { get; init; } = string.Empty;
}

public record CreateUserRequest
{
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public Role Role { get; init; }
}

public record UpdateUserRequest
{
    public Guid Id { get; init; }
    public string? Login { get; init; } = string.Empty;
    public string? Password { get; init; } = string.Empty;
    public Role? Role { get; init; }
}