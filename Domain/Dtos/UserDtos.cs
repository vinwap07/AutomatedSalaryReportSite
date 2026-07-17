using Domain.Entities;

namespace Domain.Dtos;

public record UserDetailsDto(
    Guid Id,
    string Login);

public record UserListItemDto(
    Guid Id,
    string Login);

public record CreateUserRequest(
    string Login,
    string Password,
    Role Role);

public record UpdateUserRequest(
    Guid Id,
    string Login,
    string Password,
    Role Role);