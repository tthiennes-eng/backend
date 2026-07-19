using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Domain.Repositories;

public interface IWaitListRepository
{
    Task<IEnumerable<WaitListEntry>> GetActiveByClinicAsync(Guid clinicId);
    Task<IEnumerable<WaitListEntry>> GetActiveBySpecialtyAsync(Specialty specialty);
    Task<WaitListEntry?> GetByIdAsync(Guid id);
    Task AddAsync(WaitListEntry entry);
    Task UpdateAsync(WaitListEntry entry);
}
