using Livy.Domain.Entities;

namespace Livy.Application.Interfaces;

public interface IUserRepository
{
    // Obtener usuario por ID (útil para perfiles o administración)
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Buscar por Email (fundamental para el proceso de Login)
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    // Verificar si un email ya está registrado (para el Registro)
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken ct = default);

    // Guardar un nuevo usuario
    Task AddAsync(User user, CancellationToken ct = default);

    // Actualizar datos del usuario (nombre, contraseña, rol)
    Task UpdateAsync(User user, CancellationToken ct = default);

    // Listado de usuarios (solo para roles de Admin)
    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
}