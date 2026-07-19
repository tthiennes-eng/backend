using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Api.Controllers;

/// <summary>
/// Controller para gestão de agendamentos e horários da clínica.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IAppointmentRepository appointmentRepository, ILogger<AppointmentsController> logger)
    {
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Recupera agendamentos em um intervalo de tempo.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByRange([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] Guid? clinicId = null)
    {
        var appointments = await _appointmentRepository.GetByRangeAsync(start, end, clinicId);
        return Ok(appointments);
    }

    /// <summary>
    /// Cria um novo agendamento.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Appointment appointment)
    {
        await _appointmentRepository.AddAsync(appointment);
        return CreatedAtAction(nameof(GetByRange), new { start = appointment.StartTime, end = appointment.StartTime }, appointment);
    }

    /// <summary>
    /// Atualiza o status de um agendamento.
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        appointment.UpdateStatus(request.Status);
        await _appointmentRepository.UpdateAsync(appointment);

        return NoContent();
    }

    /// <summary>
    /// Reagenda uma consulta.
    /// </summary>
    [HttpPut("{id}/reschedule")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleRequest request)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        appointment.Reschedule(request.StartTime, request.EndTime);
        await _appointmentRepository.UpdateAsync(appointment);

        return NoContent();
    }
}

public record UpdateStatusRequest(AppointmentStatus Status);
public record RescheduleRequest(DateTime StartTime, DateTime EndTime);
