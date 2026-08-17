// Application/Services/IUserService.cs
using Livy.Application.DTOs.User;

namespace Livy.Application.Services;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken ct = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}