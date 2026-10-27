// Interfaces/IUnitOfWork.cs 
namespace Livy.Domain.Interfaces; 
 
public interface IUnitOfWork 
{ 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); 
} 