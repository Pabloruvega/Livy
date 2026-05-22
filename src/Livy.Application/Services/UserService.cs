// Application/Services/UserService.cs
using Livy.Application.DTOs.User;
using Livy.Application.Contracts.Persistence;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Livy.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Livy.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repo,
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger)
    {
        _repo       = repo;
        _unitOfWork = unitOfWork;
        _logger     = logger;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Registrando usuario con email {Email}.", dto.Email);

        var isUnique = await _repo.IsEmailUniqueAsync(dto.Email, ct);
        if (!isUnique)
            throw new DomainException($"El email '{dto.Email}' ya está registrado.");

        var hash = HashPassword(dto.Password);
        var user = new User(dto.Email, dto.Name, hash);

        await _repo.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct); // ← persiste en la BD

        _logger.LogInformation("Usuario registrado con Id {Id}.", user.Id);
        return MapToDto(user);
    }

    public async Task<UserDto?> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Intento de login para {Email}.", dto.Email);

        var user = await _repo.GetByEmailAsync(dto.Email, ct);
        if (user is null)
        {
            _logger.LogWarning("Login fallido: email {Email} no encontrado.", dto.Email);
            return null;
        }

        var hash = HashPassword(dto.Password);
        if (user.PasswordHash != hash)
        {
            _logger.LogWarning("Login fallido: contraseña incorrecta para {Email}.", dto.Email);
            return null;
        }

        _logger.LogInformation("Login exitoso para {Email}.", dto.Email);
        return MapToDto(user);
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

    // SHA-256 — en producción usar BCrypt o ASP.NET Identity
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private static UserDto MapToDto(User u) => new()
    {
        Id        = u.Id,
        Email     = u.Email,
        Name      = u.Name,
        Role      = u.Role,
        CreatedAt = u.CreatedAt
    };
}