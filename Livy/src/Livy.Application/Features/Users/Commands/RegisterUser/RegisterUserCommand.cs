// Application/Features/Users/Commands/RegisterUser/RegisterUserCommand.cs
using Livy.Application.DTOs.User;
using MediatR;

namespace Livy.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Name,
    string Password
) : IRequest<UserDto>;
