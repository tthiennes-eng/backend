using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Domain.Repositories;

/// <summary>
/// Interface para o repositório de usuários.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Busca um usuário pelo email.
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca um usuário pelo ID.
    /// </summary>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Lista usuários com filtros opcionais.
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync(UserRole? role = null, string? search = null);

    /// <summary>
    /// Persiste as alterações de um usuário.
    /// </summary>
    Task UpdateAsync(User user);

    /// <summary>
    /// Salva um novo usuário.
    /// </summary>
    Task AddAsync(User user);
}
