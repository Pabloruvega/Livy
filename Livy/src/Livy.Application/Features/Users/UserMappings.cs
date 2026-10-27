// Application/Features/Users/UserMappings.cs
using Livy.Application.DTOs.User;
using Livy.Domain.Entities;

namespace Livy.Application.Features.Users;

// Único mapeo User → UserDto, compartido por todos los handlers de Users.
public static class UserMappings
{
    public static UserDto ToDto(this User user) => new()
    {
        Id        = user.Id,
        Email     = user.Email.Value,
        Name      = user.Name,
        Role      = user.Role,
        CreatedAt = user.CreatedAt
    };
}
