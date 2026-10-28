// Application/Contracts/External/IPasswordHasher.cs
namespace Livy.Application.Contracts.External;

// Application solo conoce el contrato; el algoritmo (BCrypt) es un detalle de Infrastructure.
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
