using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class LogAuditoriaConfiguration : IEntityTypeConfiguration<LogAuditoria>
{
    public void Configure(EntityTypeBuilder<LogAuditoria> builder)
    {
        builder.ToTable("logs_auditoria");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");

        builder.Property(l => l.UsuarioId).HasColumnName("usuario_id");
        builder.Property(l => l.Acao).HasColumnName("acao").IsRequired().HasMaxLength(100);
        builder.Property(l => l.Recurso).HasColumnName("recurso").HasMaxLength(100);

        builder.Property(l => l.Detalhes)
            .HasColumnName("detalhes")
            .HasColumnType("jsonb");

        builder.Property(l => l.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
        builder.Property(l => l.UserAgent).HasColumnName("user_agent");
        builder.Property(l => l.DataHora).HasColumnName("data_hora").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(l => l.Usuario)
            .WithMany()
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
