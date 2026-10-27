// Application/DTOs/User/RegisterUserDto.cs
namespace Livy.Application.DTOs.User;

// La validación vive en RegisterUserCommandValidator (FluentValidation).
public class RegisterUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
