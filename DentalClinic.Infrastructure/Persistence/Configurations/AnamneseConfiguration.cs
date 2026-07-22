using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class AnamneseConfiguration : IEntityTypeConfiguration<Anamnese>
{
    public void Configure(EntityTypeBuilder<Anamnese> builder)
    {
        builder.ToTable("anamneses");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");

        builder.Property(a => a.PatientId).HasColumnName("paciente_id");
        builder.Property(a => a.CriadoPorId).HasColumnName("criado_por_id");

        builder.Property(a => a.RespostasJson)
            .HasColumnName("respostas_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(a => a.CreatedAt).HasColumnName("criado_em");
        builder.Property(a => a.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.CriadoPor)
            .WithMany()
            .HasForeignKey(a => a.CriadoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.PatientId).IsUnique();
    }
}
