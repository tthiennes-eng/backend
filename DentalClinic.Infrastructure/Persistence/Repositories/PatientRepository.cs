using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de pacientes utilizando Entity Framework Core.
/// </summary>
public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(int page, int pageSize, string? searchTerm)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.FullName.ToLower().Contains(searchTerm) ||
                p.CPF.Contains(searchTerm));
        }

        return await query
            .OrderBy(p => p.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Patient patient)
    {
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patient patient)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync(string? searchTerm)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.FullName.ToLower().Contains(searchTerm) ||
                p.CPF.Contains(searchTerm));
        }

        return await query.CountAsync();
    }

    public async Task AnonymizeAsync(Guid id)
    {
        var patient = await GetByIdAsync(id);
        if (patient != null)
        {
            patient.FullName = "Paciente Anonimizado";
            patient.Email = $"anonimo_{patient.Id}@anonimo.com";
            patient.Phone = "***";
            patient.IsActive = false;
            patient.UpdatedAt = DateTime.UtcNow;
            
            await UpdateAsync(patient);
        }
    }
}
