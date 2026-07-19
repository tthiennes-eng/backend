using DentalClinic.Api.Filters;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de auditoria para conformidade com a LGPD.
/// </summary>
public class LogAuditoriaRepository : ILogAuditoriaRepository
{
    private readonly ApplicationDbContext _context;

    public LogAuditoriaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LogAuditoria>> GetLogsAsync(int page, int pageSize, string? userId = null)
    {
        var query = _context.Set<LogAuditoria>().AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            var userGuid = Guid.Parse(userId);
            query = query.Where(l => l.UsuarioId == userGuid);
        }

        return await query
            .OrderByDescending(l => l.DataHora)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(string? userId = null)
    {
        var query = _context.Set<LogAuditoria>().AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            var userGuid = Guid.Parse(userId);
            query = query.Where(l => l.UsuarioId == userGuid);
        }

        return await query.CountAsync();
    }
}
