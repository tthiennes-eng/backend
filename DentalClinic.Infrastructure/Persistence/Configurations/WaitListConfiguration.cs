using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class WaitListConfiguration : IEntityTypeConfiguration<WaitListEntry>
{
    public void Configure(EntityTypeBuilder<WaitListEntry> builder)
    {
        builder.ToTable("lista_espera");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Priority)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Specialty)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId);

        builder.HasOne(e => e.Clinic)
            .WithMany()
            .HasForeignKey(e => e.ClinicId);

        builder.HasIndex(e => e.IsResolved);
    }
}
