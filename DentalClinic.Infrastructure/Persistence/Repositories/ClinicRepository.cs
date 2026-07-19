using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de clínicas.
/// </summary>
public class ClinicRepository : IClinicRepository
{
    private readonly ApplicationDbContext _context;

    public ClinicRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Clinic>> GetAllAsync()
    {
        return await _context.Set<Clinic>()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Clinic?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Clinic>().FindAsync(id);
    }

    public async Task AddAsync(Clinic clinic)
    {
        await _context.Set<Clinic>().AddAsync(clinic);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Clinic clinic)
    {
        _context.Set<Clinic>().Update(clinic);
        await _context.SaveChangesAsync();
    }
}
