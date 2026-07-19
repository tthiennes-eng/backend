using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de procedimentos.
/// </summary>
public class ProcedureRepository : IProcedureRepository
{
    private readonly ApplicationDbContext _context;

    public ProcedureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Procedure>> GetAllAsync()
    {
        return await _context.Set<Procedure>()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Procedure>> GetBySpecialtyAsync(Specialty specialty)
    {
        return await _context.Set<Procedure>()
            .Where(p => p.Specialty == specialty)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Procedure?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Procedure>().FindAsync(id);
    }

    public async Task AddAsync(Procedure procedure)
    {
        await _context.Set<Procedure>().AddAsync(procedure);
        await _context.SaveChangesAsync();
    }
}
