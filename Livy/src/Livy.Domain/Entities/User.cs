// ECommerce.Domain/Entities/User.cs
namespace Livy.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;  // nunca en DTOs
    public string Role { get; private set; } = "User";  // "User" | "Admin"
    public DateTime CreatedAt { get; private set; }
 
    private User() { }
    public User(string email, string name, string passwordHash)
    {
        Email        = email;
        Name         = name;
        PasswordHash = passwordHash;
        CreatedAt    = DateTime.UtcNow;
    }

    public void PromoteToAdmin() => Role = "Admin";
}
