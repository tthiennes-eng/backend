using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.ToTable("clinicas");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.Name).HasColumnName("nome").IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasColumnName("descricao");

        builder.Property(c => c.Specialty)
            .HasColumnName("especialidade")
            .HasConversion<int>();

        builder.Property(c => c.IsActive).HasColumnName("ativo").HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasColumnName("criado_em");
        builder.Property(c => c.UpdatedAt).HasColumnName("atualizado_em");

        // Ignora propriedades que não estão no SQL original
        builder.Ignore(c => c.Address);
        builder.Ignore(c => c.OpeningTime);
        builder.Ignore(c => c.ClosingTime);
        builder.Ignore(c => c.MaxCapacity);
        builder.Ignore(c => c.CoordinatorUserId);
    }
}
