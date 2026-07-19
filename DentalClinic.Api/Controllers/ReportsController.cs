using DentalClinic.Core.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Api.Controllers;

/// <summary>
/// Controller para fornecer dados estatísticos e relatórios gerenciais.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportRepository reportRepository, ILogger<ReportsController> logger)
    {
        _reportRepository = reportRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtém as estatísticas principais para o Dashboard.
    /// </summary>
    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var stats = await _reportRepository.GetDashboardStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas do dashboard.");
            return StatusCode(500, "Erro interno ao processar estatísticas.");
        }
    }

    /// <summary>
    /// Obtém o relatório de produção por profissional em um período.
    /// </summary>
    [HttpGet("production")]
    [Authorize(Roles = "Admin,Coordinator")]
    public async Task<IActionResult> GetProductionReport([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var report = await _reportRepository.GetProductionByProfessionalAsync(start, end);
        return Ok(report);
    }
}
