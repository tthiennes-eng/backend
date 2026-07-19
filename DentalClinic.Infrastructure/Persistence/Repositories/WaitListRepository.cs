using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de lista de espera.
/// Gerencia a fila de pacientes por clínica e especialidade.
/// </summary>
public class WaitListRepository : IWaitListRepository
{
    private readonly ApplicationDbContext _context;

    public WaitListRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WaitListEntry>> GetActiveByClinicAsync(Guid clinicId)
    {
        return await _context.Set<WaitListEntry>()
            .Include(e => e.Patient)
            .Where(e => e.ClinicId == clinicId && !e.IsResolved)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WaitListEntry>> GetActiveBySpecialtyAsync(Specialty specialty)
    {
        return await _context.Set<WaitListEntry>()
            .Include(e => e.Patient)
            .Where(e => e.Specialty == specialty && !e.IsResolved)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<WaitListEntry?> GetByIdAsync(Guid id)
    {
        return await _context.Set<WaitListEntry>()
            .Include(e => e.Patient)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task AddAsync(WaitListEntry entry)
    {
        await _context.Set<WaitListEntry>().AddAsync(entry);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WaitListEntry entry)
    {
        _context.Set<WaitListEntry>().Update(entry);
        await _context.SaveChangesAsync();
    }
}
