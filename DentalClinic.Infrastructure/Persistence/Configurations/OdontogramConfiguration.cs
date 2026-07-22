using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class OdontogramConfiguration : IEntityTypeConfiguration<Odontogram>
{
    public void Configure(EntityTypeBuilder<Odontogram> builder)
    {
        builder.ToTable("odontogramas");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("id");

        builder.Property(o => o.PatientId).HasColumnName("paciente_id");

        builder.Property(o => o.Teeth)
            .HasColumnName("dados_dentes_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(o => o.UpdatedBy).HasColumnName("atualizado_por");
        builder.Property(o => o.CreatedAt).HasColumnName("criado_em");
        builder.Property(o => o.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasOne(o => o.Patient)
            .WithMany()
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.PatientId).IsUnique();
    }
}
