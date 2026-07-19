using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Domain.Repositories;

/// <summary>
/// Interface para o repositório de prontuários.
/// Gerencia Odontogramas, Evoluções, Prescrições, Atestados e Anamnese.
/// </summary>
public interface IProntuarioRepository
{
    // Odontograma
    Task<Odontogram?> GetOdontogramByPatientIdAsync(Guid patientId);
    Task SaveOdontogramAsync(Odontogram odontogram);

    // Evoluções
    Task AddEvolutionAsync(Guid patientId, string description, Guid professorId, Guid studentId);
    Task<IEnumerable<Evolution>> GetEvolutionHistoryAsync(Guid patientId);
    Task<Evolution?> GetEvolutionByIdAsync(Guid id);
    Task UpdateEvolutionAsync(Evolution evolution);

    // Documentos
    Task<Prescription> CreatePrescriptionAsync(Prescription prescription);
    Task<IEnumerable<Prescription>> GetPrescriptionHistoryAsync(Guid patientId);
    Task<MedicalCertificate> CreateCertificateAsync(MedicalCertificate certificate);

    // Anamnese
    Task<Anamnese?> GetAnamneseByPatientIdAsync(Guid patientId);
    Task SaveAnamneseAsync(Anamnese anamnese);
}
