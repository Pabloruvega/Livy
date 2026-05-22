// Application/Interfaces/IUserRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;

namespace Livy.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default);
}