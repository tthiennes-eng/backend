using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class EvolutionConfiguration : IEntityTypeConfiguration<Evolution>
{
    public void Configure(EntityTypeBuilder<Evolution> builder)
    {
        builder.ToTable("evolucoes_clinicas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.IsSignedByProfessor)
            .HasDefaultValue(false);

        builder.HasOne(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Professor)
            .WithMany()
            .HasForeignKey(e => e.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice para busca rápida de histórico do paciente
        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.CreatedAt);
    }
}
