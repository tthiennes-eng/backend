using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

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

        // Mapeamento de Endereço (Objeto C# -> Coluna JSONB conforme SQL)
        builder.Property(p => p.Address)
            .HasColumnName("endereco_json")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<DentalClinic.Core.Domain.ValueObjects.Address>(v, (JsonSerializerOptions)null!)
            );

        builder.Property(p => p.LgpdConsent)
            .HasColumnName("consentimento_lgpd")
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAt).HasColumnName("criado_em");
        builder.Property(p => p.UpdatedAt).HasColumnName("atualizado_em");

        builder.Ignore(p => p.IsActive);

        builder.HasIndex(p => p.FullName);
    }
}
