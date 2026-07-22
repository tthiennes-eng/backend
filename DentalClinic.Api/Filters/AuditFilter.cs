using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using DentalClinic.Core.Domain.Entities;
using DentalClinic.Infrastructure.Persistence;
using System.Text.Json;

namespace DentalClinic.Api.Filters;

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

        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return;

        var path = context.HttpContext.Request.Path.ToString();
        var method = context.HttpContext.Request.Method;

        if (method != "GET" || path.Contains("prontuario") || path.Contains("odontogram"))
        {
            try
            {
                var auditLog = new DentalClinic.Core.Domain.Entities.LogAuditoria
                {
                    Usuario = userId,
                    Acao = $"{method} {path}",
                    Tabela = path.Split('/').Skip(2).FirstOrDefault() ?? "API",
                    IpOrigem = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    DataHora = DateTime.UtcNow,
                    DadosNovos = JsonSerializer.Serialize(new
                    {
                        Query = context.HttpContext.Request.QueryString.ToString(),
                        StatusCode = context.HttpContext.Response.StatusCode
                    })
                };

                _context.LogsAuditoria.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Audit bypass: {msg}", ex.Message);
            }
        }
    }
}
