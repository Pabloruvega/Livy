// Application/Contracts/Persistence/IProductRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;

namespace Livy.Application.Contracts.Persistence;


public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken ct = default);
    Task<IEnumerable<Product>> SearchByNameAsync(string term, bool includeInactive, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
} 