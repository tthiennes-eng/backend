using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

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
            .OrderByDescending(p => p.CreatedAt) // Mostra os mais recentes primeiro
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Patient patient)
    {
        // Se o Id vier vazio do Flutter (improvável), gera um novo
        if (patient.Id == Guid.Empty)
        {
            patient.Id = Guid.NewGuid();
        }

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
            patient.FullName = "PACIENTE ANONIMIZADO (LGPD)";
            patient.CPF = "00000000000";
            patient.Email = "anonimo@odontoclinica.edu.br";
            patient.Phone = "0000000000";
            patient.Address = null;
            
            await UpdateAsync(patient);
        }
    }
}
