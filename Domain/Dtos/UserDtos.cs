using Domain.Entities; 

namespace Domain.Dtos;

public record UserDetailsDto
{
    public Guid Id { get; init; }
    public string Login { get; init; } = string.Empty;
    
    public Role Role { get; init; }
}

public record UserListItemDto
{
    public Guid Id { get; init; }
    public string Login { get; init; } = string.Empty;
    public Role Role { get; init; }
}

public record CreateUserRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Role Role { get; set; }
}

public record UpdateUserRequest
{
    public Guid Id { get; set; }

    /// <summary>Null — оставить прежний логин</summary>
    public string? Login { get; set; }

    /// <summary>Null — оставить прежний пароль</summary>
    public string? Password { get; set; }

    /// <summary>Null — оставить прежнюю роль</summary>
    public Role? Role { get; set; }
}