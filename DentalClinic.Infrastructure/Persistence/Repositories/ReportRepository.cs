using DentalClinic.Core.Domain.Repositories;
using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de relatórios e indicadores utilizando dados reais do Banco de Dados.
/// </summary>
public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var totalPatients = await _context.Patients.CountAsync();

        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        var appointmentsToday = await _context.Appointments
            .CountAsync(a => a.StartTime.Date == today && a.Status != AppointmentStatus.Cancelled);

        var proceduresThisMonth = await _context.Set<TreatmentItem>()
            .CountAsync(i => i.Status == TreatmentItemStatus.Completed && i.UpdatedAt >= firstDayOfMonth);

        var pendingAlerts = await _context.Evolutions
            .CountAsync(e => !e.IsSignedByProfessor);

        // Busca dados reais de crescimento dos últimos 6 meses
        var growthData = new List<MonthlyGrowth>();
        for (int i = 5; i >= 0; i--)
        {
            var monthDate = today.AddMonths(-i);
            var start = new DateTime(monthDate.Year, monthDate.Month, 1);
            var end = start.AddMonths(1);

            var count = await _context.Appointments
                .CountAsync(a => a.StartTime >= start && a.StartTime < end && a.Status == AppointmentStatus.Completed);

            growthData.Add(new MonthlyGrowth(monthDate.ToString("MMM"), count));
        }

        return new DashboardStats(
            totalPatients,
            appointmentsToday,
            proceduresThisMonth,
            pendingAlerts,
            growthData
        );
    }

    public async Task<IEnumerable<ProductionReport>> GetProductionByProfessionalAsync(DateTime start, DateTime end)
    {
        // Agrega o valor total de itens de tratamento concluídos por profissional (Professor)
        var report = await _context.Evolutions
            .Where(e => e.CreatedAt >= start && e.CreatedAt <= end && e.IsSignedByProfessor)
            .GroupBy(e => e.Professor.Name)
            .Select(g => new ProductionReport(
                g.Key,
                "Professor",
                g.Count(),
                0 // Valor seria calculado se TreatmentItem estivesse vinculado diretamente à evolução
            ))
            .ToListAsync();

        return report;
    }
}
