// Application/DTOs/User/LoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace Livy.Application.DTOs.User;

/// <summary>Request: credenciales para autenticarse</summary>
public class LoginDto
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}