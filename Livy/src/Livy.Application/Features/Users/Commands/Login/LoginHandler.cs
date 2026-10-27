// Application/Features/Users/Commands/Login/LoginHandler.cs
using Livy.Application.Contracts.External;
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.User;
using Livy.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Users.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponseDto?>
{
    private readonly IUserRepository  _repo;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        IUserRepository repo,
        IJwtTokenService jwtTokenService,
        ILogger<LoginHandler> logger)
    {
        _repo            = repo;
        _jwtTokenService = jwtTokenService;
        _logger          = logger;
    }

    // Email inexistente y contraseña incorrecta devuelven lo mismo (null) para no
    // revelar qué emails están registrados; solo el log distingue el motivo.
    public async Task<AuthResponseDto?> Handle(LoginCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Intento de login para {Email}.", request.Email);

        var user = await _repo.GetByEmailAsync(Email.Create(request.Email), ct);
        if (user is null)
        {
            _logger.LogWarning("Login fallido: email {Email} no encontrado.", request.Email);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login fallido: contraseña incorrecta para {Email}.", request.Email);
            return null;
        }

        _logger.LogInformation("Login exitoso para {Email}.", request.Email);

        return new AuthResponseDto
        {
            Token   = _jwtTokenService.GenerateToken(user),
            Email   = user.Email.Value,
            Name    = user.Name,
            Role    = user.Role,
            Expires = _jwtTokenService.GetExpiration()
        };
    }
}
