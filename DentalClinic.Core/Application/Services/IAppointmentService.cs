using DentalClinic.Core.Application.DTOs;
using DentalClinic.Core.Common;
using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Application.Services;

/// <summary>
/// Interface para o serviço de agendamentos, contendo regras de negócio complexas.
/// </summary>
public interface IAppointmentService
{
    /// <summary>
    /// Tenta agendar uma nova consulta, verificando conflitos de horário.
    /// </summary>
    Task<Result<Appointment>> ScheduleAsync(Appointment appointment);

    /// <summary>
    /// Verifica se há conflito de horário para um profissional ou consultório.
    /// </summary>
    Task<bool> HasConflictAsync(Guid doctorId, DateTime start, DateTime end);

    /// <summary>
    /// Move um agendamento para a lista de espera.
    /// </summary>
    Task AddToWaitListAsync(Guid patientId, Guid clinicId, string procedure);

    /// <summary>
    /// Cancela uma consulta seguindo as regras de auditoria.
    /// </summary>
    Task<Result<bool>> CancelAsync(Guid appointmentId, string reason);
}
