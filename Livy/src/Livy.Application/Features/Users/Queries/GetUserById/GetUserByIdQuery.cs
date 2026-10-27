// Application/Features/Users/Queries/GetUserById/GetUserByIdQuery.cs
using Livy.Application.DTOs.User;
using MediatR;

namespace Livy.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;
