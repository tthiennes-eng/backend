using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Domain.Repositories;

/// <summary>
/// Interface para persistência de sessões de usuário (Refresh Tokens).
/// </summary>
public interface IUserSessionRepository
{
    Task<UserSession?> GetByTokenAsync(string token);
    Task AddAsync(UserSession session);
    Task UpdateAsync(UserSession session);
    Task RevokeAllUserSessionsAsync(Guid userId);
}
