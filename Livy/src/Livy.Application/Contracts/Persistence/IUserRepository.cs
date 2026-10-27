// Application/Contracts/Persistence/IUserRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;
using Livy.Domain.ValueObjects;

namespace Livy.Application.Contracts.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken ct = default);
}