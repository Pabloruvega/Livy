// Application/Contracts/External/IJwtTokenService.cs
using Livy.Domain.Entities;

namespace Livy.Application.Contracts.External;


public interface IJwtTokenService
{
    string GenerateToken(User user);
    DateTime GetExpiration();
}