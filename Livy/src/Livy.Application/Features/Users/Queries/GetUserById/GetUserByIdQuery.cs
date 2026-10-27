// Application/Features/Users/Queries/GetUserById/GetUserByIdQuery.cs
using Livy.Application.DTOs.User;
using MediatR;

namespace Livy.Application.Features.Users.Queries.GetUserById;

// RequestingUserId e IsAdmin salen del token: cada usuario se ve a sí mismo, el Admin ve a cualquiera.
public record GetUserByIdQuery(Guid UserId, Guid RequestingUserId, bool IsAdmin) : IRequest<UserDto>;
