using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly ApplicationDbContext _context;

    public UserSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserSession?> GetByTokenAsync(string token)
    {
        return await _context.Set<UserSession>()
            .FirstOrDefaultAsync(s => s.Token == token);
    }

    public async Task AddAsync(UserSession session)
    {
        await _context.Set<UserSession>().AddAsync(session);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserSession session)
    {
        _context.Set<UserSession>().Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserSessionsAsync(Guid userId)
    {
        var sessions = await _context.Set<UserSession>()
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.Revoke();
        }

        await _context.SaveChangesAsync();
    }
}
