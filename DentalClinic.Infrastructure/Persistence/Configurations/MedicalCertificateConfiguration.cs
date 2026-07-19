using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class MedicalCertificateConfiguration : IEntityTypeConfiguration<MedicalCertificate>
{
    public void Configure(EntityTypeBuilder<MedicalCertificate> builder)
    {
        builder.ToTable("atestados");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.CID)
            .HasMaxLength(10);

        builder.Property(c => c.DaysOfRest)
            .IsRequired();

        builder.HasOne(c => c.Patient)
            .WithMany()
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Doctor)
            .WithMany()
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.PatientId);
    }
}
