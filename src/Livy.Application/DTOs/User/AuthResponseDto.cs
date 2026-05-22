// Application/DTOs/User/AuthResponseDto.cs
namespace Livy.Application.DTOs.User;

/// <summary>Response del login: token JWT + datos básicos del usuario.</summary>
public class AuthResponseDto
{
    public string Token     { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string Name      { get; set; } = string.Empty;
    public string Role      { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}