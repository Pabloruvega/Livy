// Application/DTOs/User/LoginDto.cs
namespace Livy.Application.DTOs.User;

/// <summary>Request: credenciales para autenticarse (validación en LoginCommandValidator)</summary>
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
