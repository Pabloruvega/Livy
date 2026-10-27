// Application/Features/Users/Queries/GetUserById/GetUserByIdHandler.cs
using Livy.Application.Common.Exceptions;
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.User;
using Livy.Domain.Exceptions;
using MediatR;

namespace Livy.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _repo;

    public GetUserByIdHandler(IUserRepository repo) => _repo = repo;

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct)
            ?? throw new UserNotFoundException(request.UserId);

        if (!request.IsAdmin && user.Id != request.RequestingUserId)
            throw new ForbiddenAccessException("No tenés permiso para ver este usuario.");

        return user.ToDto();
    }
}
