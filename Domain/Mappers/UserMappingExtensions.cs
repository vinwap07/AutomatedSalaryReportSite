using Domain.Dtos;
using Domain.Entities;
using Mapster;

namespace Domain.Mappers;

public static class UserMappingExtensions
{
    public static UserDetailsDto ToDetailsDto(this User user)
    {
        return user.Adapt<UserDetailsDto>();
    }

    public static UserListItemDto ToListItemDto(this User user)
    {
        return user.Adapt<UserListItemDto>();
    }

    public static User ToUser(this CreateUserRequest request)
    {
        return request.Adapt<User>();
    }

    public static void UpdateUser(this UpdateUserRequest user, User existingUser)
    {
        user.Adapt(existingUser);
    }
}