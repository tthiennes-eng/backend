using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("pacientes");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.FullName)
            .HasColumnName("nome_completo")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.CPF)
            .HasColumnName("cpf")
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(p => p.CPF).IsUnique();

        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasMaxLength(255);

        builder.Property(p => p.Phone)
            .HasColumnName("telefone")
            .HasMaxLength(20);

        builder.Property(p => p.BirthDate)
            .HasColumnName("data_nascimento")
            .IsRequired();

        builder.Property(p => p.Gender)
            .HasColumnName("sexo")
            .HasMaxLength(1);

        // Mapeamento de Endereço como JSONB conforme DATABASE_SCHEMA.sql
        builder.Property(p => p.Address)
            .HasColumnName("endereco_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.LgpdConsent)
            .HasColumnName("consentimento_lgpd")
            .HasDefaultValue(false);

        builder.Property(p => p.IsActive)
            .HasColumnName("ativo")
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt).HasColumnName("criado_em");
        builder.Property(p => p.UpdatedAt).HasColumnName("atualizado_em");

        builder.HasIndex(p => p.FullName);
    }
}
