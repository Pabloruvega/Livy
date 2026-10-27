// Application/Contracts/Persistence/ICategoryRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;

namespace Livy.Application.Contracts.Persistence;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> ExistsAsync(string name, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}