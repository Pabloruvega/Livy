// Application/Interfaces/ICategoryRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;

namespace Livy.Application.Interfaces;

/// <summary>
/// Extiende IRepository&lt;Category&gt; con operaciones específicas de categorías.
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> ExistsAsync(string name, CancellationToken ct = default);
}