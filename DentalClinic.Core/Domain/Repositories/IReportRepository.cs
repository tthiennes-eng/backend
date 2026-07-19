namespace DentalClinic.Core.Domain.Repositories;

/// <summary>
/// Interface para geração de indicadores e relatórios gerenciais.
/// </summary>
public interface IReportRepository
{
    Task<DashboardStats> GetDashboardStatsAsync();
    Task<IEnumerable<ProductionReport>> GetProductionByProfessionalAsync(DateTime start, DateTime end);
}

public record DashboardStats(
    int TotalPatients,
    int AppointmentsToday,
    int ProceduresThisMonth,
    int PendingAlerts,
    List<MonthlyGrowth> GrowthData
);

public record MonthlyGrowth(string Month, int Count);
public record ProductionReport(string ProfessionalName, string Role, int TotalProcedures, decimal TotalValue);
