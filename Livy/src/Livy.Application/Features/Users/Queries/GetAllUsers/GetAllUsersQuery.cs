// Application/Features/Users/Queries/GetAllUsers/GetAllUsersQuery.cs
using Livy.Application.DTOs.User;
using MediatR;

namespace Livy.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery() : IRequest<IEnumerable<UserDto>>;
