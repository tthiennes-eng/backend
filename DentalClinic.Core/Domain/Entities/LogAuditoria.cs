using System;

namespace DentalClinic.Core.Domain.Entities;

public class LogAuditoria
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UsuarioId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? Recurso { get; set; }
    public string? Detalhes { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;

    // Relacionamento opcional com o usuário
    public User? Usuario { get; set; }
}
