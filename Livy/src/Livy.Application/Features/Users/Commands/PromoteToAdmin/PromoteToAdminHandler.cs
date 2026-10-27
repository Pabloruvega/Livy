// Application/Features/Users/Commands/PromoteToAdmin/PromoteToAdminHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.User;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Users.Commands.PromoteToAdmin;

public class PromoteToAdminHandler : IRequestHandler<PromoteToAdminCommand, UserDto>
{
    private readonly IUserRepository _repo;
    private readonly IUnitOfWork     _unitOfWork;
    private readonly ILogger<PromoteToAdminHandler> _logger;

    public PromoteToAdminHandler(
        IUserRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<PromoteToAdminHandler> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<UserDto> Handle(PromoteToAdminCommand request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.UserId, ct)
            ?? throw new UserNotFoundException(request.UserId);

        user.PromoteToAdmin();
        await _repo.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Usuario {Id} promovido a Admin.", user.Id);
        return user.ToDto();
    }
}
