// Application/Features/Users/Commands/RegisterUser/RegisterUserHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.User;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using Livy.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _repo;
    private readonly IUnitOfWork     _unitOfWork;
    private readonly ILogger<RegisterUserHandler> _logger;

    public RegisterUserHandler(
        IUserRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUserHandler> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Registrando usuario con email {Email}.", request.Email);

        var email = Email.Create(request.Email);  // valida y normaliza (trim + minúsculas)

        var isUnique = await _repo.IsEmailUniqueAsync(email, ct);
        if (!isUnique)
            throw new DomainException($"El email '{email.Value}' ya está registrado.");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(email, request.Name, hash);

        await _repo.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Usuario registrado con Id {Id}.", user.Id);
        return user.ToDto();
    }
}
