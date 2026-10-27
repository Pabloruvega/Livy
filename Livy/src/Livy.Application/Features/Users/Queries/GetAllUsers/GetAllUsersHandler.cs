// Application/Features/Users/Queries/GetAllUsers/GetAllUsersHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.User;
using MediatR;

namespace Livy.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _repo;

    public GetAllUsersHandler(IUserRepository repo) => _repo = repo;

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var users = await _repo.GetAllAsync(ct);
        return users.Select(u => u.ToDto());
    }
}
