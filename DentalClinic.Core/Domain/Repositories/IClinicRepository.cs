using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Domain.Repositories;

public interface IClinicRepository
{
    Task<IEnumerable<Clinic>> GetAllAsync();
    Task<Clinic?> GetByIdAsync(Guid id);
    Task AddAsync(Clinic clinic);
    Task UpdateAsync(Clinic clinic);
}
