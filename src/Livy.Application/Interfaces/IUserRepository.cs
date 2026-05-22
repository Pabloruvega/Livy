// Application/Interfaces/IUserRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;

namespace Livy.Application.Interfaces;

/// <summary>
/// Extiende IRepository&lt;User&gt; con operaciones específicas de usuarios.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
}