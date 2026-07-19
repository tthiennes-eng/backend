using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using DentalClinic.Core.Domain.Entities;
using DentalClinic.Infrastructure.Persistence;
using System.Text.Json;

namespace DentalClinic.Api.Filters;

/// <summary>
/// Filtro global de auditoria para conformidade com a LGPD.
/// Registra automaticamente quem acessou ou modificou dados sensíveis.
/// </summary>
public class AuditFilter : IAsyncActionFilter
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuditFilter> _logger;

    public AuditFilter(ApplicationDbContext context, ILogger<AuditFilter> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        // Só registra logs de sucesso ou se for uma operação de modificação/acesso a prontuário
        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return;

        var path = context.HttpContext.Request.Path.ToString();
        var method = context.HttpContext.Request.Method;

        // Identifica ações sensíveis (ex: GET /api/patients/{id}/odontogram)
        if (method != "GET" || path.Contains("prontuario") || path.Contains("odontogram"))
        {
            var auditLog = new LogAuditoria
            {
                UsuarioId = Guid.Parse(userId),
                Acao = $"{method} {path}",
                IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                DataHora = DateTime.UtcNow,
                Detalhes = JsonSerializer.Serialize(new
                {
                    Query = context.HttpContext.Request.QueryString.ToString(),
                    StatusCode = context.HttpContext.Response.StatusCode
                })
            };

            _context.Set<LogAuditoria>().Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}

public class LogAuditoria : Entity
{
    public Guid UsuarioId { get; set; }
    public string Acao { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public DateTime DataHora { get; set; }
    public string? Detalhes { get; set; }
}
