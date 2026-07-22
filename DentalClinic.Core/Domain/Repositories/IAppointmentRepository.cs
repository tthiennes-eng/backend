using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Domain.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Appointment>> GetByRangeAsync(DateTime start, DateTime end, Guid? clinicId);
    Task AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
}
