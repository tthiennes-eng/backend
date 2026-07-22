using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using DentalClinic.Core.Domain.Entities;
using DentalClinic.Infrastructure.Persistence;
using System.Text.Json;

namespace DentalClinic.Api.Filters;

/// <summary>
/// Filtro global de auditoria para conformidade com a LGPD.
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

        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return;

        var path = context.HttpContext.Request.Path.ToString();
        var method = context.HttpContext.Request.Method;

        // Registra apenas ações de escrita ou acessos a dados sensíveis
        if (method != "GET" || path.Contains("prontuario") || path.Contains("odontogram"))
        {
            try
            {
                var auditLog = new DentalClinic.Core.Domain.Entities.LogAuditoria
                {
                    UsuarioId = userId,
                    Acao = $"{method} {path}",
                    Recurso = path.Split('/').Skip(2).FirstOrDefault() ?? "API",
                    IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                    DataHora = DateTime.UtcNow,
                    Detalhes = JsonSerializer.Serialize(new
                    {
                        StatusCode = context.HttpContext.Response.StatusCode,
                        Query = context.HttpContext.Request.QueryString.ToString()
                    })
                };

                _context.LogsAuditoria.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Falha silenciosa na auditoria: {Message}", ex.Message);
            }
        }
    }
}
