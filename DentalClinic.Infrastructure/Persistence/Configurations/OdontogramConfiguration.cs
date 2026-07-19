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

        builder.Property(o => o.Teeth)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasOne(o => o.Patient)
            .WithMany()
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.PatientId).IsUnique();
        builder.HasIndex(o => o.UpdatedAt);
    }
}
