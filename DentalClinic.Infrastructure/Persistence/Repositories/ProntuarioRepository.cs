using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

public class ProntuarioRepository : IProntuarioRepository
{
    private readonly ApplicationDbContext _context;

    public ProntuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Odontogram?> GetOdontogramByPatientIdAsync(Guid patientId)
    {
        return await _context.Odontograms
            .FirstOrDefaultAsync(o => o.PatientId == patientId);
    }

    public async Task SaveOdontogramAsync(Odontogram odontogram)
    {
        var existing = await GetOdontogramByPatientIdAsync(odontogram.PatientId);
        if (existing == null)
            _context.Odontograms.Add(odontogram);
        else
            _context.Entry(existing).CurrentValues.SetValues(odontogram);

        await _context.SaveChangesAsync();
    }

    public async Task AddEvolutionAsync(Guid patientId, string description, Guid professorId, Guid studentId)
    {
        var clinic = await _context.Clinics.FirstOrDefaultAsync();
        var clinicId = clinic?.Id ?? Guid.Empty;

        var evolution = Evolution.Create(
            patientId,
            studentId,
            professorId,
            clinicId,
            description
        );

        _context.Evolutions.Add(evolution);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Evolution>> GetEvolutionHistoryAsync(Guid patientId)
    {
        return await _context.Evolutions
            .Include(e => e.Professor)
            .Include(e => e.Student)
            .Where(e => e.PatientId == patientId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Evolution?> GetEvolutionByIdAsync(Guid id)
    {
        return await _context.Evolutions.FindAsync(id);
    }

    public async Task UpdateEvolutionAsync(Evolution evolution)
    {
        _context.Evolutions.Update(evolution);
        await _context.SaveChangesAsync();
    }

    public async Task<Prescription> CreatePrescriptionAsync(Prescription prescription)
    {
        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
        return prescription;
    }

    public async Task<IEnumerable<Prescription>> GetPrescriptionHistoryAsync(Guid patientId)
    {
        return await _context.Prescriptions
            .Where(p => p.PatientId == patientId)
            .ToListAsync();
    }

    public async Task<MedicalCertificate> CreateCertificateAsync(MedicalCertificate certificate)
    {
        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync();
        return certificate;
    }

    public async Task<Anamnese?> GetAnamneseByPatientIdAsync(Guid patientId)
    {
        return await _context.Anamneses
            .FirstOrDefaultAsync(a => a.PatientId == patientId);
    }

    public async Task SaveAnamneseAsync(Anamnese anamnese)
    {
        var existing = await GetAnamneseByPatientIdAsync(anamnese.PatientId);
        if (existing == null)
            _context.Anamneses.Add(anamnese);
        else
            _context.Entry(existing).CurrentValues.SetValues(anamnese);

        await _context.SaveChangesAsync();
    }
}
