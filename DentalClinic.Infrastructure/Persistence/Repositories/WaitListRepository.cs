using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

public class WaitListRepository : IWaitListRepository
{
    private readonly ApplicationDbContext _context;

    public WaitListRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WaitListEntry>> GetActiveByClinicAsync(Guid clinicId)
    {
        return await _context.WaitListEntries
            .Include(w => w.Patient)
            .Where(w => w.ClinicId == clinicId && !w.IsResolved)
            .OrderBy(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WaitListEntry>> GetActiveBySpecialtyAsync(DentalClinic.Core.Domain.ValueObjects.Specialty specialty)
    {
        return await _context.WaitListEntries
            .Include(w => w.Patient)
            .Where(w => w.Specialty == specialty && !w.IsResolved)
            .OrderBy(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<WaitListEntry?> GetByIdAsync(Guid id)
    {
        return await _context.WaitListEntries
            .Include(w => w.Patient)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task AddAsync(WaitListEntry entry)
    {
        await _context.WaitListEntries.AddAsync(entry);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WaitListEntry entry)
    {
        _context.WaitListEntries.Update(entry);
        await _context.SaveChangesAsync();
    }

    public async Task ResolveEntryAsync(Guid entryId)
    {
        var entry = await _context.WaitListEntries.FindAsync(entryId);
        if (entry != null)
        {
            entry.Resolve();
            await _context.SaveChangesAsync();
        }
    }
}
