// Application/Interfaces/IProductRepository.cs
using Livy.Domain.Entities;
using Livy.Domain.Interfaces;

namespace Livy.Application.Interfaces;


public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> SearchByNameAsync(string term, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}