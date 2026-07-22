using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DentalClinic.Core.Domain.Repositories;

namespace DentalClinic.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public DashboardController(IPatientRepository patientRepository, IAppointmentRepository appointmentRepository)
    {
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        // Retorna dados reais baseados no banco ou valores mockados para destravar a UI
        var totalPatients = await _patientRepository.CountAsync(null);
        var today = DateTime.UtcNow.Date;
        var appointments = await _appointmentRepository.GetByRangeAsync(today, today.AddDays(1), null);

        return Ok(new
        {
            TotalPatients = totalPatients,
            AppointmentsToday = appointments.Count(),
            ProceduresThisMonth = 45, // Exemplo mockado
            PendingAlerts = 2,        // Exemplo mockado
            GrowthData = new[]
            {
                new { Month = "Jan", Count = 10 },
                new { Month = "Fev", Count = 15 },
                new { Month = "Mar", Count = 22 }
            }
        });
    }
}
