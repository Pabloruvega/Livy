// Application/DTOs/User/UserDto.cs
namespace Livy.Application.DTOs.User;

/// <summary>Response: datos públicos del usuario (NUNCA incluir PasswordHash)</summary>
public class UserDto
{
    public Guid     Id        { get; set; }
    public string   Email     { get; set; } = string.Empty;
    public string   Name      { get; set; } = string.Empty;
    public string   Role      { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}