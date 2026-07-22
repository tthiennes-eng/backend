using DentalClinic.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinic.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // Nome da tabela conforme DATABASE_SCHEMA.sql
        builder.ToTable("pacientes");

        // Chave Primária
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

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

        // Mapeamento de Endereço (Objeto C# -> Coluna JSONB no Postgres)
        // Usamos .ToJson() se for EF Core 7+, ou mapeamos como propriedade simples com tipo jsonb
        builder.Property(p => p.Address)
            .HasColumnName("endereco_json")
            .HasColumnType("jsonb");

        builder.Property(p => p.LgpdConsent)
            .HasColumnName("consentimento_lgpd")
            .HasDefaultValue(false);

        // Caso a coluna 'ativo' não exista no seu banco, ignore a propriedade IsActive do mapeamento
        builder.Ignore(p => p.IsActive);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("criado_em")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("atualizado_em")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Índices para busca performance
        builder.HasIndex(p => p.FullName);
    }
}
