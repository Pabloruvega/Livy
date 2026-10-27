// Application/Services/UserService.cs
using Livy.Application.DTOs.User;
using Livy.Application.Contracts.External;
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using Livy.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repo,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        ILogger<UserService> logger)
    {
        _repo            = repo;
        _unitOfWork      = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _logger          = logger;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Registrando usuario con email {Email}.", dto.Email);

        var email = Email.Create(dto.Email);  // valida y normaliza (trim + minúsculas)

        var isUnique = await _repo.IsEmailUniqueAsync(email, ct);
        if (!isUnique)
            throw new DomainException($"El email '{email.Value}' ya está registrado.");

        var hash = HashPassword(dto.Password);
        var user = new User(email, dto.Name, hash);

        await _repo.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct); // ← persiste en la BD

        _logger.LogInformation("Usuario registrado con Id {Id}.", user.Id);
        return MapToDto(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Intento de login para {Email}.", dto.Email);

        var user = await _repo.GetByEmailAsync(Email.Create(dto.Email), ct);
        if (user is null)
        {
            _logger.LogWarning("Login fallido: email {Email} no encontrado.", dto.Email);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login fallido: contraseña incorrecta para {Email}.", dto.Email);
            return null;
        }

        _logger.LogInformation("Login exitoso para {Email}.", dto.Email);

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResponseDto
        {
            Token   = token,
            Email   = user.Email.Value,
            Name    = user.Name,
            Role    = user.Role,
            Expires = _jwtTokenService.GetExpiration()
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _repo.GetAllAsync(ct);
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(id, ct);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto> PromoteToAdminAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _repo.GetByIdAsync(userId, ct)
            ?? throw new UserNotFoundException(userId);

        user.PromoteToAdmin();
        await _repo.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Usuario {Id} promovido a Admin.", user.Id);
        return MapToDto(user);
    }

    private static string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    private static UserDto MapToDto(User u) => new()
    {
        Id        = u.Id,
        Email     = u.Email.Value,
        Name      = u.Name,
        Role      = u.Role,
        CreatedAt = u.CreatedAt
    };
}