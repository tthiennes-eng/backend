using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Domain.Repositories;

public interface IProcedureRepository
{
    Task<IEnumerable<Procedure>> GetAllAsync();
    Task<IEnumerable<Procedure>> GetBySpecialtyAsync(Specialty specialty);
    Task<Procedure?> GetByIdAsync(Guid id);
    Task AddAsync(Procedure procedure);
}
