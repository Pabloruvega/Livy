// Application/Features/Users/Commands/PromoteToAdmin/PromoteToAdminCommand.cs
using Livy.Application.DTOs.User;
using MediatR;

namespace Livy.Application.Features.Users.Commands.PromoteToAdmin;

public record PromoteToAdminCommand(Guid UserId) : IRequest<UserDto>;
