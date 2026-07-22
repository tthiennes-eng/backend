using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class WaitListConfiguration : IEntityTypeConfiguration<WaitListEntry>
{
    public void Configure(EntityTypeBuilder<WaitListEntry> builder)
    {
        builder.ToTable("lista_espera");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).HasColumnName("id");

        builder.Property(w => w.PatientId).HasColumnName("paciente_id");
        builder.Property(w => w.ClinicId).HasColumnName("clinica_id");

        builder.Property(w => w.Specialty)
            .HasColumnName("especialidade")
            .HasConversion<int>();

        builder.Property(w => w.Priority).HasColumnName("prioridade").HasMaxLength(50);
        builder.Property(w => w.Observation).HasColumnName("observacao");
        builder.Property(w => w.IsResolved).HasColumnName("resolvido").HasDefaultValue(false);
        builder.Property(w => w.CreatedAt).HasColumnName("criado_em");

        builder.HasOne(w => w.Patient)
            .WithMany()
            .HasForeignKey(w => w.PatientId);

        builder.HasOne(w => w.Clinic)
            .WithMany()
            .HasForeignKey(w => w.ClinicId);
    }
}
